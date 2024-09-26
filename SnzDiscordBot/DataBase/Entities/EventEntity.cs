using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("events")]
[Index(nameof(GuildId), nameof(ChannelId),nameof(MessageId))]
public class EventEntity : BaseEntity
{
    public EventEntity(ulong guildId, ulong channelId, ulong messageId)
    {
        GuildId = guildId;
        ChannelId = channelId;
        MessageId = messageId;
    }

    public ulong MessageId { get; private set; }

    public ulong GuildId { get; private set; }

    public ulong ChannelId { get; private set; }
    
    public DateTime StartAt { get; set; } = DateTime.MinValue;
    
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)] 
    public string Description { get; set; } = string.Empty;
}
