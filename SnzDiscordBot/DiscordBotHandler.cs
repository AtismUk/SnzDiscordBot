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
using SnzDiscordBot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SnzDiscordBot;

internal class DiscordBotHandler : IHostedService
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly IConfiguration _config;
    private readonly CommandHandler _commandHandler;
    private readonly ILogger<DiscordBotHandler> _logger;

    public DiscordBotHandler(DiscordSocketClient client, InteractionService intService, IConfiguration config, CommandHandler commandHandler, ILogger<DiscordBotHandler> logger)
    {
        _discordSocketClient = client;
        _interactionService = intService;
        _config = config;
        _commandHandler = commandHandler;
        _logger = logger;
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
        
        _logger.Log(logLevel, message.Exception, $"{message.Source}: {message.Message}");
        
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var client = _discordSocketClient;
        var commands = _interactionService;

        client.Log += Log;
        client.Ready += async () =>
        {
            await commands.RegisterCommandsGloballyAsync();
        };
        await client.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
        await client.StartAsync();

        await _commandHandler.InitializeAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordSocketClient.StopAsync();
    }
}


