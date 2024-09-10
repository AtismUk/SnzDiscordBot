using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

namespace SnzDiscordBot;

class Program
{
    private static DiscordSocketClient _client;
    private InteractionService _commands;
    
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<DiscordSocketClient>(new DiscordSocketClient(new()
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
                }));
                services.AddSingleton(x => new InteractionService(x.GetService<DiscordSocketClient>()));
                services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                        .Build());
                services.AddSingleton<CommandHandler>();

                services.AddSingleton<DiscordBotHandler>();
            }).Build();

        var discordBot = host.Services.GetService<DiscordBotHandler>();

        await discordBot!.RunAsync();
    }
}