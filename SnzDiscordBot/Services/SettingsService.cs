
using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class SettingsService : ISettingsService
{
    private readonly IBaseRepo _dbRepo;
    
    public SettingsService(IBaseRepo dbRepo)
    {
        _dbRepo = dbRepo;
    }
    
    public async Task<SettingsEntity> GetSettingsAsync(ulong guildId)
    {
        var dbSet  = _dbRepo.DbContext.Set<SettingsEntity>();
        var settings = await dbSet.FirstOrDefaultAsync(x => x.GuildId == guildId);
        if (settings == null)
        {
            settings = new SettingsEntity() { GuildId = guildId };
            await _dbRepo.AddUpdateEntityAsync(settings);
        }
        return settings;
    }

    public async Task<bool> SaveSettingsAsync(ulong guildId, 
        ulong? auditChannelId = null, 
        ulong? applicationChannelId = null, 
        ulong? applicationAddRoleId = null, 
        ulong? applicationRemoveRoleId = null, 
        ulong? eventChannelId = null, 
        ulong? newsChannelId = null, 
        ulong? scheduleChannelId = null)
    {
        var settings = await GetSettingsAsync(guildId);

        settings.AuditChannelId = auditChannelId ?? settings.AuditChannelId;
        settings.ApplicationChannelId = applicationChannelId ?? settings.ApplicationChannelId;
        settings.ApplicationAddRoleId = applicationAddRoleId ?? settings.ApplicationAddRoleId;
        settings.ApplicationRemoveRoleId = applicationRemoveRoleId ?? settings.ApplicationRemoveRoleId;
        settings.EventChannelId = eventChannelId ?? settings.EventChannelId;
        settings.NewsChannelId = newsChannelId ?? settings.NewsChannelId;
        settings.ScheduleChannelId = scheduleChannelId ?? settings.ScheduleChannelId;
        
        return await _dbRepo.AddUpdateEntityAsync(settings);
    }
}