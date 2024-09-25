using System.ComponentModel.DataAnnotations.Schema;

namespace SnzDiscordBot.DataBase.Entities;

[Table("events")]
public class EventEntity : BaseEntity
{
    public required ulong ChannelId { get; set; }
    public required ulong MessageId { get; set; }
    public required DateTimeOffset DateTime { get; set; }
    public required DateTime StartTime { get; set; }
    public List<ulong> VotedYesUserIds { get; set; } = [];
    public List<ulong> VotedNoUserIds { get; set; } = [];
    public List<ulong> VotedMaybeUserIds { get; set; } = [];
    public List<ulong> Came { get; set; } = [];
}