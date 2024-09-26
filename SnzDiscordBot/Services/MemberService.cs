using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class MemberService : IMemberService
{
    private readonly IBaseRepo _baseRepo;

    public MemberService(IBaseRepo baseRepo)
    {
        _baseRepo = baseRepo;
    }

    public async Task<MemberEntity?> GetMemberAsync(ulong guildId, ulong userId)
    {
        // Пытаемся найти первую подходящую запись или создаем новую.
        return await _baseRepo.FirstOrDefaultAsync<MemberEntity>(s => s.GuildId == guildId && s.UserId == userId);
    }

    public async Task<MemberEntity?> UpdateMemberAsync(ulong guildId, ulong userId, string? username = null, Rank? rank = null, Group? group = null, List<Role>? roles = null, Status? status = null)
    {
        // Пытаемся найти существующую запись для изменения или создаем новую.
        var member = await GetMemberAsync(guildId, userId) ?? new MemberEntity(guildId, userId);
        
        // Обновляем данные
        member.Username = username ?? member.Username;
        member.Rank = rank ?? member.Rank;
        member.Group = group ?? member.Group;
        member.Status = status ?? member.Status;
        member.Roles = roles ?? member.Roles;
        
        // Передаем в BaseRepo
        return await _baseRepo.AddUpdateEntityAsync(member);
    }
}