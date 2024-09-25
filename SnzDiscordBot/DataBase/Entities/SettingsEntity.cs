using System.ComponentModel.DataAnnotations.Schema;

namespace SnzDiscordBot.DataBase.Entities;

[Table("settings")]
public class SettingsEntity : BaseEntity
{
    public ulong AuditChannelId { get; set; } = 0;
    public ulong ApplicationChannelId { get; set; } = 0;
    public ulong ApplicationAddRoleId { get; set; } = 0;
    public ulong ApplicationRemoveRoleId { get; set; } = 0;
    public ulong EventChannelId { get; set; } = 0;
    public ulong NewsChannelId { get; set; } = 0;
    public ulong ScheduleChannelId { get; set; } = 0;
    public ulong StaffChannelId { get; set; } = 0;
}