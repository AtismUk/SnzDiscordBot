using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("settings")]
[Index(nameof(GuildId))]
public class SettingsEntity : BaseEntity
{
    public SettingsEntity(ulong guildId)
    {
        GuildId = guildId;
    }

    public ulong GuildId { get; private set; }
    
    public ulong AuditChannelId { get; set; } = 0;
    
    public ulong ApplicationChannelId { get; set; } = 0;
    
    public ulong ApplicationAddRoleId { get; set; } = 0;
    
    public ulong ApplicationRemoveRoleId { get; set; } = 0;
    
    public ulong EventsChannelId { get; set; } = 0;
    
    public ulong NewsChannelId { get; set; } = 0;
    
    public ulong SchedulesChannelId { get; set; } = 0;
}