using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("members")]
[PrimaryKey(nameof(GuildId), nameof(UserId))]
[Index(nameof(GuildId), nameof(UserId))]
public class MemberEntity : BaseEntity
{
    public MemberEntity(ulong guildId, ulong userId)
    {
        UserId = userId;
        GuildId = guildId;
    }
    
    public ulong GuildId { get; private set; }
    public ulong UserId { get; private set; }
    [MaxLength(100)] public string Username { get; set; } = "Не определен";
    public Rank Rank { get; set; } = Rank.Unknown;
    public Group Group { get; set; } = Group.Unknown;
    public List<Role> Roles { get; set; } = [];
    public Status Status { get; set; } = Status.Unknown;
}


public enum Rank
{
    Unknown = 0,
    Rookie,
    Private,
    Corporal,
    JuniorSergeant,
    Sergeant,
    SeniorSergeant,
    Foreman,
    Ensign,
    SeniorEnsign,
    JuniorLieutenant,
    Lieutenant,
    SeniorLieutenant,
    Captain,
    Major,
    LieutenantColonel,
    Colonel
}

public enum Group
{
    Unknown = 0,
    First,
    Second,
    Third,
    Fourth, 
    Fifth, 
    Sixth,
    TransportSupport,
    FireSupport,
    Recon,
    Leadership
}

public enum Role
{
    Commander,
    DeputyCommander,
    HumanResources,
    Instructor,
    Creative,
    Technical,
    Founder
}

public enum Status
{
    Unknown = 0,
    Active,
    Vacation,
    Reserve,
    Left,
}