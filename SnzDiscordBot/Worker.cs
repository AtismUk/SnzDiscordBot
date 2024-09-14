using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SnzDiscordBot;

public class Worker : BackgroundService
{
    private static ILogger<Worker> _logger = default!;
    private static DiscordSocketClient _client = default!;
    private InteractionService _commands = default!;
        
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Получение всех сервисов по DI
        var services = ConfigureServices();
                
        //Настройка Discord
        _client = services.GetRequiredService<DiscordSocketClient>();
        _commands = services.GetRequiredService<InteractionService>();
        _client.Log += Log;
        
        _client.Ready += async () =>
        {
            await _commands.RegisterCommandsGloballyAsync();
        };
        await _client.LoginAsync(TokenType.Bot, services.GetRequiredService<IConfiguration>()["Discord:Token"]);
        await _client.StartAsync();

        await services.GetRequiredService<CommandHandler>().InitializeAsync();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken);
        }
    }

    public ServiceProvider ConfigureServices()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
        };
        
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
            .Build();

        return new ServiceCollection()
            .AddSingleton<DiscordSocketClient>(x => new DiscordSocketClient(config))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<CommandHandler>()
            .AddSingleton<IConfiguration>(configuration)
            .BuildServiceProvider();
    }

    static Task Log(LogMessage message)
    {
        var log = $"{message.Source}: {message.Message}. {message.Exception}";
        switch (message.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical(log);
                break;
            case LogSeverity.Error:
                _logger.LogError(log);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning(log);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug(log);
                break;
            default:
                _logger.LogInformation(log);
                break;
        }
        return Task.CompletedTask;
    }
}