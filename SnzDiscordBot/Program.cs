using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;

namespace SnzDiscordBot;

class Program
{
    private static DiscordSocketClient _client;
    private InteractionService _commands;
    
    static async Task Main(string[] args) => await new Program().RunAsync();

    public async Task RunAsync()
    {

        // Получение всех сервисов по DI
        var services = ConfigureServices();
                
        //Настройка Disocrd
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
        await Task.Delay(-1);

    }

    public ServiceProvider ConfigureServices()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
        };

        IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName)
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
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}