using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("member_awards")]
[Index(nameof(GuildId), nameof(UserId), nameof(AwardDescriptor))]
public class MemberAwardEntity : BaseEntity
{
    public MemberAwardEntity(string awardDescriptor, ulong userId, ulong guildId, DateTime awardDate)
    {
        AwardDescriptor = awardDescriptor;
        UserId = userId;
        GuildId = guildId;
        AwardDate = awardDate;
    }
    public ulong GuildId { get; private set; }
    
    [MaxLength(50)]
    [ForeignKey(nameof(AwardEntity.Descriptor))] public string AwardDescriptor { get; private set; }
    
    [ForeignKey(nameof(MemberEntity.UserId))] public ulong UserId { get; private set; }

    public DateTime AwardDate { get; private set; }
    
    [MaxLength(500)]
    public string AwardReason { get; set; } = "Неизвестно";
}