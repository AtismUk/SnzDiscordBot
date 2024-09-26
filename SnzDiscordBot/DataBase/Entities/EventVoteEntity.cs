using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("event_votes")]
[Index(nameof(EventMessageId), nameof(UserId), IsUnique = true)]
public class EventVoteEntity : BaseEntity
{
    public EventVoteEntity(ulong eventMessageId, ulong userId, ulong guildId)
    {
        EventMessageId = eventMessageId;
        UserId = userId;
        GuildId = guildId;
    }

    public ulong GuildId { get; }
    [ForeignKey(nameof(EventEntity.MessageId))]
    public ulong EventMessageId { get; }
    [ForeignKey(nameof(MemberEntity.UserId))]
    public ulong UserId { get; }
    public VoteType Type { get; set; } = VoteType.Unknown;

}

public enum VoteType
{
    Unknown,
    Yes,
    No,
    Maybe
}