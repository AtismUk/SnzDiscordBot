using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface ISettingsService
{
    Task<SettingsEntity?> GetSettingsAsync(ulong guildId);

    Task<SettingsEntity?> UpdateSettingsAsync(ulong guildId,
        ulong? auditChannelId = null, ulong? applicationChannelId = null, ulong? applicationAddRoleId = null,
        ulong? applicationRemoveRoleId = null, ulong? newsChannelId = null, ulong? eventsChannelId = null,
        ulong? scheduleChannelId = null, bool requesterIsGetter = false);
}