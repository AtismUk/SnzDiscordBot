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

namespace SnzDiscordBot
{
    internal class DiscordBotHandler
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly InteractionService _interactionService;
        private readonly IConfiguration _config;
        private readonly CommandHandler _commandHandler;
        public DiscordBotHandler(DiscordSocketClient client, InteractionService intService, IConfiguration config, CommandHandler commandHandler)
        {
            _discordSocketClient = client;
            _interactionService = intService;
            _config = config;
            _commandHandler = commandHandler;
        }

        public async Task RunAsync()
        {

            //Настройка Disocrd
            var _client = _discordSocketClient;
            var _commands = _interactionService;

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

        static Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
