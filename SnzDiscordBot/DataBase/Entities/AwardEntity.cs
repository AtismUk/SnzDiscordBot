using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SnzDiscordBot.DataBase.Entities;

[Table("awards")]
[Index(nameof(Descriptor), IsUnique = true)]
public class AwardEntity : BaseEntity
{
    public AwardEntity(ulong guildId, string descriptor)
    {
        Descriptor = descriptor;
        GuildId = guildId;
    }
    
    public ulong GuildId { get; }
    [MaxLength(50)] public string Descriptor { get; }
    public int Priority { get; set; } = 0;
    [MaxLength(1000)] public string Name { get; set; } = "Не определено";
    [MaxLength(1000)] public string Description { get; set; } = "Не определено";
    [MaxLength(1000)] public string ImageUrl { get; set; } = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/PlaceholderLC.png/180px-PlaceholderLC.png";
}
