using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class MemberService : IMemberService
{
    private readonly IBaseRepo _dbRepo;
    
    public MemberService(IBaseRepo dbRepo)
    {
        _dbRepo = dbRepo;
    }
    
    public async Task<MemberEntity?> GetMemberAsync(ulong guildId, ulong userId)
    {
        var dbSet  = _dbRepo.DbContext.Set<MemberEntity>();
        var member = await dbSet.FirstOrDefaultAsync(x => x.UserId == userId && x.GuildId == guildId);
        return member;
    }

    public async Task<bool> AddUpdateMemberAsync(ulong guildId, ulong userId, string? username, Rank? rank, Group? group, Role[]? roles, Status? status)
    {
        var member = await GetMemberAsync(guildId, userId);
        
        if (member == null)
            return false;
        
        member.Username = username ?? member.Username;
        member.Rank = rank ?? member.Rank;
        member.Group = group ?? member.Group;
        member.Roles = roles ?? member.Roles;
        member.Status = status ?? member.Status;
        
        return await _dbRepo.AddUpdateEntityAsync(member);
    }
}