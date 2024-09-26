using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class SettingsService : ISettingsService
{
    private readonly IBaseRepo _baseRepo;
    
    public SettingsService(IBaseRepo baseRepo)
    {
        _baseRepo = baseRepo;
    }
    
    public async Task<SettingsEntity?> GetSettingsAsync(ulong guildId)
    {
        // Пытаемся найти первую подходящую запись или создаем новую.
        return await _baseRepo.FirstOrDefaultAsync<SettingsEntity>(s => s.GuildId == guildId) ?? await UpdateSettingsAsync(guildId, requesterIsGetter: true);
    }

    public async Task<SettingsEntity?> UpdateSettingsAsync(ulong guildId, ulong? auditChannelId = null, ulong? applicationChannelId = null, ulong? applicationAddRoleId = null, ulong? applicationRemoveRoleId = null, ulong? newsChannelId = null, ulong? eventsChannelId = null, ulong? scheduleChannelId = null, bool requesterIsGetter = false)
    {
        // Пытаемся найти существующую для изменения или создаем новую.
        SettingsEntity settings;
        if (!requesterIsGetter)
            settings = await GetSettingsAsync(guildId) ?? new SettingsEntity(guildId);
        else
            settings = new SettingsEntity(guildId);
        
        // Обновляем данные
        settings.AuditChannelId = auditChannelId ?? settings.AuditChannelId;
        settings.ApplicationChannelId = applicationChannelId ?? settings.ApplicationChannelId;
        settings.ApplicationAddRoleId = applicationAddRoleId ?? settings.ApplicationAddRoleId;
        settings.ApplicationRemoveRoleId = applicationRemoveRoleId ?? settings.ApplicationRemoveRoleId;
        settings.NewsChannelId = newsChannelId ?? settings.NewsChannelId;
        settings.EventsChannelId = eventsChannelId ?? settings.EventsChannelId;
        settings.SchedulesChannelId = scheduleChannelId ?? settings.SchedulesChannelId;
            
        // Передаем в BaseRepo
        return await _baseRepo.AddUpdateEntityAsync(settings);
    }
}