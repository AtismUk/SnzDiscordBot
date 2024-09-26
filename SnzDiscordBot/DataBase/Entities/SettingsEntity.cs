using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("settings")]
[Index(nameof(GuildId), IsUnique = true)]
public class SettingsEntity : BaseEntity
{
    public SettingsEntity(ulong guildId)
    {
        GuildId = guildId;
    }

    [Key, Column(Order = 1)] public ulong GuildId { get; }
    
    public ulong AuditChannelId { get; set; } = 0;
    
    public ulong ApplicationChannelId { get; set; } = 0;
    
    public ulong ApplicationAddRoleId { get; set; } = 0;
    
    public ulong ApplicationRemoveRoleId { get; set; } = 0;
    
    public ulong EventsChannelId { get; set; } = 0;
    
    public ulong NewsChannelId { get; set; } = 0;
    
    public ulong SchedulesChannelId { get; set; } = 0;
}