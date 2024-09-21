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
        
        resultBuilder.AppendLine($"Канал аудита: <#{_config["Settings:Audit_Channel_Id"]}>");
        resultBuilder.AppendLine("## Настройки регистрации");
        resultBuilder.AppendLine($"Канал регистрации: <#{_config["Settings:Application_channel_Id"]}>");
        resultBuilder.AppendLine($"Удаляемая роль при рег.: <@&{_config["Settings:Remove_Application_Role_Id"]}>");
        resultBuilder.AppendLine($"Выдаваемая роль при рег.: <@&{_config["Settings:Add_Application_Role_Id"]}>");
        resultBuilder.AppendLine("## Настройки оповещений");
        resultBuilder.AppendLine($"Канал новостей: <#{_config["Settings:News_Channel_Id"]}>");
        resultBuilder.AppendLine($"Канал мероприятий: <#{_config["Settings:Event_Channel_Id"]}>");
        resultBuilder.AppendLine($"Канал расписания: <#{_config["Settings:Schedule_Channel_Id"]}>");
        
        await RespondAsync(resultBuilder.ToString(), ephemeral: true);
    }
}