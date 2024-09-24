using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface ISettingsService
{
    Task<SettingsEntity> GetSettingsAsync(ulong guildId);
    Task<bool> SaveSettingsAsync(ulong guildId, 
        ulong? auditChannelId = null, 
        ulong? applicationChannelId = null, 
        ulong? applicationAddRoleId = null, 
        ulong? applicationRemoveRoleId = null, 
        ulong? eventChannelId = null, 
        ulong? newsChannelId = null, 
        ulong? scheduleChannelId = null);
}