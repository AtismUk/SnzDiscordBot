using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IMemberService
{
    Task<MemberEntity?> GetMemberAsync(ulong guildId, ulong userId);
    
    Task<MemberEntity?> UpdateMemberAsync(ulong guildId, ulong userId, string? username = null, Rank? rank = null, Group? group = null, List<Role>? roles = null, Status? status = null);
}