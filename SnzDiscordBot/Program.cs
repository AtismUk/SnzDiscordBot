using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SnzDiscordBot;

class Program
{
    static async Task Main()
    {
        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
                }));
                services.AddSingleton(x => new InteractionService(x.GetService<DiscordSocketClient>()));
                services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                        .Build());
                services.AddSingleton<CommandHandler>();

                services.AddHostedService<DiscordBotHandler>();
            }).ConfigureLogging(x =>
            {
                x.AddConsole();
            })
            .Build()
            .RunAsync();
    }
}