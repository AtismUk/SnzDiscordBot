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
            await Task.Delay(1000, stoppingToken);
        }
    }

    public ServiceProvider ConfigureServices()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
        };

        var path = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(path)
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
        _logger.LogInformation($"[{message.Severity}] {message.Source}: {message.Message}");
        return Task.CompletedTask;
    }
}