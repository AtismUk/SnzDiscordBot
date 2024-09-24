using System.Text;
using Discord.Interactions;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Modules;

public class InfoModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ISettingsService _settingsService;
    
    public InfoModule(ISettingsService settings)
    {
        _settingsService = settings;
    }
    
    [SlashCommand("info", "Показывает настройки бота")]
    public async Task InfoCommand()
    {
        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        
        var resultBuilder = new StringBuilder();
        
        resultBuilder.AppendLine($"Канал аудита: <#{settings.AuditChannelId}>");
        resultBuilder.AppendLine("## Настройки регистрации");
        resultBuilder.AppendLine($"Канал регистрации: <#{settings.ApplicationChannelId}>");
        resultBuilder.AppendLine($"Удаляемая роль при рег.: <@&{settings.ApplicationRemoveRoleId}>");
        resultBuilder.AppendLine($"Выдаваемая роль при рег.: <@&{settings.ApplicationAddRoleId}>");
        resultBuilder.AppendLine("## Настройки оповещений");
        resultBuilder.AppendLine($"Канал новостей: <#{settings.NewsChannelId}>");
        resultBuilder.AppendLine($"Канал мероприятий: <#{settings.EventChannelId}>");
        resultBuilder.AppendLine($"Канал расписания: <#{settings.ScheduleChannelId}>");
        
        await RespondAsync(resultBuilder.ToString(), ephemeral: true);
    }
}