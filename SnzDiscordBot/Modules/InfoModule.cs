using System.Text;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;

namespace SnzDiscordBot.Modules;

public class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IConfiguration _config;
    public InfoModule(IConfiguration config)
    {
        _config = config;
    }
    
    [SlashCommand("info", "Показывает настройки бота")]
    public async Task InfoCommand()
    {
        var resultBuilder = new StringBuilder();

        resultBuilder.AppendLine($"Канал регистрации: <#{_config["Settings:Application_channel_Id"]}>");
        resultBuilder.AppendLine($"Удаляемая роль при рег.: <@&{_config["Settings:Remove_Application_Role_Id"]}>");
        resultBuilder.AppendLine($"Выдаваемая роль при рег.: <@&{_config["Settings:Add_Application_Role_Id"]}>");
        
        await RespondAsync(resultBuilder.ToString());
    }
}