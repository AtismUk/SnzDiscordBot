using Discord.WebSocket;
using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IMemberService
{
    Task<MemberEntity?> GetMemberAsync(ulong guildId, ulong userId);
    Task<bool> AddUpdateMemberAsync(ulong guildId, ulong userId, string? username, Rank? rank, Group? group, Role[]? roles, Status? status);
}