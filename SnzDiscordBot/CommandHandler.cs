using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace SnzDiscordBot;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _service;
    private readonly ILogger _logger;

    public CommandHandler(DiscordSocketClient client, InteractionService interactionService, IServiceProvider service, ILogger<CommandHandler> logger)
    {
        _client = client;
        _commands = interactionService;
        _service = service;
        _logger = logger;
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

    private static async Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
    }

    private static async Task ModalCommandExecuted(ModalCommandInfo modalCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
    }

    private static async Task ContextCommandExecuted(ContextCommandInfo contextCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
    }

    private static async Task SlashCommandExecuted(SlashCommandInfo slashCommand, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            var exec = (ExecuteResult)result;
            await interactionContext.Interaction.RespondAsync($"{exec.ErrorReason}\n{exec.Exception}", ephemeral: true);
        }
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
}