using System.ComponentModel.DataAnnotations.Schema;

namespace SnzDiscordBot.DataBase.Entities;

[Table("members")]
public class MemberEntity : BaseEntity
{
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public required string Username { get; set; }
    public Rank Rank { get; set; } = Rank.Rookie;
    public Group Group { get; set; } = Group.Unknown;
    public List<Role> Roles { get; set; } = [];
    public Status Status { get; set; } = Status.Active;
}

public enum Rank
{
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
    Unknown,
    First,
    Second,
    Third,
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
    Unknown,
    Active,
    Vacation,
    Reserve
}