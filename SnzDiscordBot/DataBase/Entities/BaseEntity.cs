namespace SnzDiscordBot.DataBase.Entities;

public class BaseEntity
{
    public int Id { get; set; } = 0;
    public required ulong GuildId { get; set; }
}
