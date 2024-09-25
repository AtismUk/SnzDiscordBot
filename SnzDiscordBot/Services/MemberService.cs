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

    public async Task<bool> AddUpdateMemberAsync(ulong guildId, ulong userId, string? username = null, Rank? rank = null, Group? group = null, Status? status = null, List<Role>? roles = null)
    {
        var member = await GetMemberAsync(guildId, userId) ?? new MemberEntity
        {
            GuildId = guildId,
            UserId = userId,
            Username = "Не определен",
        };
        
        member.Username = username ?? member.Username;
        member.Rank = rank ?? member.Rank;
        member.Group = group ?? member.Group;
        member.Roles = roles ?? member.Roles;
        member.Status = status ?? member.Status;
        
        return await _dbRepo.AddUpdateEntityAsync(member);
    }

    public string GetRankName(Rank rank)
    {
        return rank switch
        {
            Rank.Rookie => "Новобранец",
            Rank.Private => "Рядовой",
            Rank.Corporal => "Ефрейтор",
            Rank.JuniorSergeant => "Мл. Сержант",
            Rank.Sergeant => "Сержант",
            Rank.SeniorSergeant => "Ст. Сержант",
            Rank.Foreman => "Старшина",
            Rank.Ensign => "Прапорщик",
            Rank.SeniorEnsign => "Ст. Прапорщик",
            Rank.JuniorLieutenant => "Лейтенант",
            Rank.Lieutenant => "Лейтенант",
            Rank.SeniorLieutenant => "Лейтенант",
            Rank.Captain => "Капитан",
            Rank.Major => "Майор",
            Rank.LieutenantColonel => "Подполковник",
            Rank.Colonel => "Полковник",
            _ => "Неизвестно"
        };
    }
    
    public string GetRoleName(Role role)
    {
        return role switch
        {
            Role.Commander => "Командир БГ",
            Role.DeputyCommander => "Зам. Командир БГ",
            Role.HumanResources => "Служба Кадров",
            Role.Instructor => "Инструктор",
            Role.Creative => "Творческий Отдел",
            Role.Technical => "Технический Отдел",
            Role.Founder => "Основатель Отряда",
            _ => "Неизвестно"
        };
    }
    
    public string GetStatusName(Status status)
    {
        return status switch
        {
            Status.Active => "Активен",
            Status.Vacation => "Отпуск",
            Status.Reserve => "Резерв",
            _ => "Неизвестно"
        };
    }
    
    public string GetGroupName(Group group)
    {
        return group switch
        {
            Group.First => "1-ая БГ \"Гранит\"",
            Group.Second => "2-ая БГ \"Топаз\"",
            Group.Third => "3-ая БГ \"Агат\"",
            Group.TransportSupport => "ГТО \"Заря\"",
            Group.FireSupport => "ГОП \"Тандем\"",
            Group.Recon => "ГСР \"Линза\"",
            Group.Leadership => "ГУ \"Полынь\"",
            _ => "Неизвестно"
        };
    }
}