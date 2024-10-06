using System.Text;
using Discord;
using Discord.Interactions;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Modules;

public class SettingsModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ISettingsService _settingsService;
    
    public SettingsModule(ISettingsService settings)
    {
        _settingsService = settings;
    }

    [SlashCommand("config", "Настройка бота")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    public async Task ConfigCommand(IChannel? audit_channel = null, IChannel? application_channel = null, IRole? application_remove_role = null, IRole? application_add_role = null, IChannel? event_channel = null, IChannel? news_channel = null, IChannel? schedule_channel = null)
    {
        if (await _settingsService.AddUpdateSettingsAsync(Context.Guild.Id, audit_channel?.Id, application_channel?.Id, application_add_role?.Id, application_remove_role?.Id, news_channel?.Id, event_channel?.Id, schedule_channel?.Id) != null)
            await RespondAsync("Данные успешно сохранены!", ephemeral: true);
        else
            await RespondAsync("Ошибка базы данных!", ephemeral: true);
    }
    
    [SlashCommand("info", "Показывает настройки бота")]
    public async Task InfoCommand()
    {
        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        if (settings == null)
        {
            await RespondAsync("Ошибка базы данных! Не удалось получить настройки.", ephemeral: true);
            return;
        }
        
        var resultBuilder = new StringBuilder();
        
        resultBuilder.AppendLine($"Канал аудита: <#{settings.AuditChannelId}>");
        resultBuilder.AppendLine("## Настройки регистрации");
        resultBuilder.AppendLine($"Канал регистрации: <#{settings.ApplicationChannelId}>");
        resultBuilder.AppendLine($"Удаляемая роль при рег.: <@&{settings.ApplicationRemoveRoleId}>");
        resultBuilder.AppendLine($"Выдаваемая роль при рег.: <@&{settings.ApplicationAddRoleId}>");
        resultBuilder.AppendLine("## Настройки оповещений");
        resultBuilder.AppendLine($"Канал новостей: <#{settings.NewsChannelId}>");
        resultBuilder.AppendLine($"Канал мероприятий: <#{settings.EventsChannelId}>");
        resultBuilder.AppendLine($"Канал расписания: <#{settings.SchedulesChannelId}>");
        
        await RespondAsync(resultBuilder.ToString(), ephemeral: true);
    }
}