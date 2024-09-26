using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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
    public async Task ConfigApplicationCommand(IChannel? audit_channel = null, IChannel? application_channel = null, IRole? application_remove_role = null, IRole? application_add_role = null, IChannel? event_channel = null, IChannel? news_channel = null, IChannel? schedule_channel = null)
    {
        if (await _settingsService.UpdateSettingsAsync(Context.Guild.Id, audit_channel?.Id, application_channel?.Id, application_add_role?.Id, application_remove_role?.Id, news_channel?.Id, event_channel?.Id, schedule_channel?.Id) != null)
            await RespondAsync("Данные успешно сохранены!", ephemeral: true);
        else
            await RespondAsync("Ошибка базы данных!", ephemeral: true);
    }
}