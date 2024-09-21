using System.Reflection;
using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SnzDiscordBot;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _service;
    private readonly ILogger _logger;
    private readonly IConfiguration _config;

    public CommandHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider service, ILogger<CommandHandler> logger, IConfiguration config)
    {
        _client = client;
        _commands = interactionService;
        _service = service;
        _logger = logger;
        _config = config;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

        _client.InteractionCreated += HandleInteraction;

        _commands.SlashCommandExecuted += SlashCommandExecuted;
        _commands.ContextCommandExecuted += ContextCommandExecuted;
        _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        _commands.ModalCommandExecuted += ModalCommandExecuted;
    }

    private async Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
        
        await LogInChannel(componentCommandInfo, interactionContext, result);
    }

    private async Task ModalCommandExecuted(ModalCommandInfo modalCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
        
        await LogInChannel(modalCommandInfo, interactionContext, result);
    }

    private async Task ContextCommandExecuted(ContextCommandInfo contextCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
        
        await LogInChannel(contextCommandInfo, interactionContext, result);
    }

    private async Task SlashCommandExecuted(SlashCommandInfo slashCommand, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }

        await LogInChannel(slashCommand, interactionContext, result);
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var socketInteractionContext = new SocketInteractionContext(_client, socketInteraction);
            await _commands.ExecuteCommandAsync(socketInteractionContext, _service);
        }
        catch (Exception e)
        {
            if (socketInteraction.Type == InteractionType.ApplicationCommand)
            {
                await socketInteraction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                
                await socketInteraction.RespondAsync(e.ToString(), ephemeral: true);
            }
        }
    }

    private async Task LogInChannel<T>(CommandInfo<T> commandInfo, IInteractionContext interactionContext, IResult result) where T : class, IParameterInfo
    {
        var auditChannel = (IMessageChannel?)await interactionContext.Guild.GetChannelAsync(ulong.Parse(_config["Settings:Audit_Channel"]!));
        if (auditChannel == null)
        {
            _logger.LogWarning($"{interactionContext.Guild.Name}: No audit channel configured!");
            return;
        }

        var commandInfoBuilder = new StringBuilder()
            .AppendLine($"Module: ```js\n{commandInfo.Module}\n```")
            .AppendLine($"Command Service: ```js\n{commandInfo.CommandService}\n```")
            .AppendLine($"Name: ```js\n{commandInfo.Name}\n```")
            .AppendLine($"Method Name: ```js\n{commandInfo.MethodName}\n```")
            .AppendLine($"Ignore Group Names: ```js\n{commandInfo.IgnoreGroupNames}\n```")
            .AppendLine($"Supports Wild Cards: ```js\n{commandInfo.SupportsWildCards}\n```")
            .AppendLine($"Is Top Level Command: ```js\n{commandInfo.IsTopLevelCommand}\n```")
            .AppendLine($"Run Mode: ```js\n{commandInfo.RunMode}\n```")
            .AppendLine($"Attributes: ```js\n{commandInfo.Attributes}\n```")
            .AppendLine($"Preconditions: ```js\n{commandInfo.Preconditions}\n```")
            .AppendLine($"Treat Name As Regex: ```js\n{commandInfo.TreatNameAsRegex}\n```")
            .AppendLine($"Parameters: ```js\n{commandInfo.Parameters}```");

        var footerBuilder = new StringBuilder()
            .AppendLine($"Channel: {interactionContext.Channel.Name}")
            .AppendLine($"Is Success: {result.IsSuccess}");

        if (!result.IsSuccess)
        {
            footerBuilder.AppendLine($"{result.Error}: {result.ErrorReason}");
        }
        
        var embedBuilder = new EmbedBuilder()
        {
            Author = new EmbedAuthorBuilder()
            {
                IconUrl = interactionContext.User.GetAvatarUrl(),
                Name = $"{interactionContext.User.Username}#{interactionContext.User.Discriminator}",
            },
            Title = typeof(T).Name,
            Description = commandInfoBuilder.ToString(),
            Footer = new EmbedFooterBuilder()
            {
                IconUrl = interactionContext.Client.CurrentUser.GetAvatarUrl(),
                Text = footerBuilder.ToString(),
            },
        };
        
        await auditChannel.SendMessageAsync(embed: embedBuilder.Build());
    }
}