using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("events")]
[Index(nameof(MessageId), nameof(GuildId), IsUnique = true)]
public class EventEntity : BaseEntity
{
    public EventEntity(ulong guildId, ulong channelId, ulong messageId)
    {
        GuildId = guildId;
        ChannelId = channelId;
        MessageId = messageId;
    }

    public ulong MessageId { get; }

    public ulong GuildId { get; }

    public ulong ChannelId { get; }
    
    public DateTime StartAt { get; set; } = DateTime.MinValue;
    
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)] 
    public string Description { get; set; } = string.Empty;
}
