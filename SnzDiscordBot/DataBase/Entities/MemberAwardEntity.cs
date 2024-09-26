using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("member_awards")]
[Index(nameof(UserId), nameof(AwardDescriptor), IsUnique = true)]
public class MemberAwardEntity : BaseEntity
{
    public MemberAwardEntity(string awardDescriptor, ulong userId, ulong guildId, DateTime awardDate)
    {
        AwardDescriptor = awardDescriptor;
        UserId = userId;
        GuildId = guildId;
        AwardDate = awardDate;
    }
    public ulong GuildId { get; }
    
    [ForeignKey(nameof(AwardEntity.Descriptor))] public string AwardDescriptor { get; }
    
    [ForeignKey(nameof(MemberEntity.UserId))] public ulong UserId { get; }

    public DateTime AwardDate { get; }
    
    [MaxLength(500)]
    public string AwardReason { get; set; } = "Неизвестно";
}