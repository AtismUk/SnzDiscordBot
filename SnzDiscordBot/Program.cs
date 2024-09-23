using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnzDiscordBot.DataBase;
using SnzDiscordBot.Services.Interfaces;
using SnzDiscordBot.Services.Repo;

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
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers | GatewayIntents.GuildMessages
                }));
                services.AddSingleton(x => new InteractionService(x.GetService<DiscordSocketClient>()));
                services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true)
                        .Build());
                services.AddSingleton<CommandHandler>();

                
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite("Data Source=DataBase/bot_bd.db");
                });
                services.AddScoped<IBaseDbRepo, BaseDbRepo>();

                services.AddHostedService<DiscordBotHandler>();
            }).ConfigureLogging(x =>
            {
                x.AddConsole();
            })
            .Build()
            .RunAsync();
    }
}