using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;

namespace SnzDiscordBot;

internal class DiscordBotHandler
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly IConfiguration _config;
    private readonly CommandHandler _commandHandler;
    private readonly ILogger<DiscordBotHandler> _logger;

    public DiscordBotHandler(
        DiscordSocketClient client, 
        InteractionService intService, 
        IConfiguration config, 
        CommandHandler commandHandler,
        ILogger<DiscordBotHandler> logger)
    {
        _discordSocketClient = client;
        _interactionService = intService;
        _config = config;
        _commandHandler = commandHandler;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var _client = _discordSocketClient;
        var _commands = _interactionService;

        // Используем метод логгирования вместо Console.WriteLine
        _client.Log += Log;
        _client.Ready += async () =>
        {
            await _commands.RegisterCommandsGloballyAsync();
        };
        await _client.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
        await _client.StartAsync();

        await _commandHandler.InitializeAsync();
        await Task.Delay(-1);
    }

    private Task Log(LogMessage message)
    {
        var logLevel = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };

        _logger.Log(logLevel, message.Exception, message.Message);

        return Task.CompletedTask;
    }
}