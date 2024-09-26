using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnzDiscordBot.DataBase.Entities;

public class BaseEntity
{
    [Key, Column(Order = 0)]
    public int Id { get; } = 0;
}
