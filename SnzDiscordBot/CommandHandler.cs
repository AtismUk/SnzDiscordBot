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

    private async Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            await interactionContext.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
        }
    }

    private async Task ModalCommandExecuted(ModalCommandInfo ModalCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            await interactionContext.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
        }
    }

    private async Task ContextCommandExecuted(ContextCommandInfo contextCommandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            await interactionContext.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
        }
    }

    private async Task SlashCommandExecuted(SlashCommandInfo slashCommand, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            await interactionContext.Interaction.RespondAsync(result.ErrorReason, ephemeral: true);
        }
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var socketInteractionContext = new SocketInteractionContext(_client, socketInteraction);
            await _commands.ExecuteCommandAsync(socketInteractionContext, _service);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            if (socketInteraction.Type == InteractionType.ApplicationCommand)
            {
                await socketInteraction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}