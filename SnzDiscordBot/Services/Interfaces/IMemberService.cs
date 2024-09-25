using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IMemberService
{
    Task<MemberEntity?> GetMemberAsync(ulong guildId, ulong userId);
    Task<bool> AddUpdateMemberAsync(ulong guildId, ulong userId, string? username = null, Rank? rank = null, Group? group = null, List<Role>? roles = null, Status? status = null);

    string GetRankName(Rank rank);
    
    string GetRoleName(Role role);

    string GetStatusName(Status status);

    string GetGroupName(Group group);
}