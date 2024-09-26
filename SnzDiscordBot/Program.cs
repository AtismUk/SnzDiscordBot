using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnzDiscordBot.DataBase;
using SnzDiscordBot.Services;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot;

class Program
{
    static async Task Main()
    {
        if (!Directory.Exists("Database"))
            Directory.CreateDirectory("Database");
        
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
                
                services.AddSingleton<IAwardService, AwardService>();
                services.AddSingleton<IEventService, EventService>();
                services.AddSingleton<IMemberService, MemberService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite("Data Source=DataBase/bot.db");
                });
                services.AddScoped<IBaseRepo, BaseRepo>();

                services.AddHostedService<DiscordBotHandler>();
            }).ConfigureLogging(x =>
            {
                x.AddConsole();
            })
            .Build()
            .RunAsync();
    }
}