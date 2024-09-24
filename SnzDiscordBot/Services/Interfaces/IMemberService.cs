using Discord.WebSocket;
using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IMemberService
{
    Task<MemberEntity?> GetMemberAsync(ulong guildId, ulong userId);
    Task<bool> AddUpdateMemberAsync(ulong guildId, ulong userId, string? username, Rank? rank, Group? group, Role[]? roles, Status? status);

    string GetRankName(Rank rank);
    
    string GetRoleName(Role role);

    string GetStatusName(Status status);

    string GetGroupName(Group group);
}