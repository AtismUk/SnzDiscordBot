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
        return await _baseRepo.FirstOrDefaultAsync<MemberEntity>(s => s.GuildId == guildId && s.UserId == userId) ?? await AddUpdateMemberAsync(guildId, userId, requesterIsGetter: true);
    }

    public async Task<MemberEntity?> AddUpdateMemberAsync(ulong guildId, ulong userId, string? username = null, Rank? rank = null, Group? group = null, List<Role>? roles = null, Status? status = null, bool requesterIsGetter = false)
    {
        // Пытаемся найти существующую запись для изменения или создаем новую.
        MemberEntity member;
        if (!requesterIsGetter)
            member = await GetMemberAsync(guildId, userId) ?? new MemberEntity(guildId, userId);
        else
            member = new MemberEntity(guildId, userId);
        
        // Обновляем данные
        member.Username = username ?? member.Username;
        member.Rank = rank ?? member.Rank;
        member.Group = group ?? member.Group;
        member.Status = status ?? member.Status;
        member.Roles = roles ?? member.Roles;
        
        // Передаем в BaseRepo
        return await _baseRepo.AddUpdateEntityAsync(member);
    }

    public static Rank StringToRank(string rankString)
    {
        return rankString.ToLower() switch
        {
            "нов" => Rank.Rookie, 
            "ряд" => Rank.Private,
            "ефр" => Rank.Corporal,
            "мсер" => Rank.JuniorSergeant,
            "сер" => Rank.Sergeant,
            "ссер" => Rank.SeniorSergeant,
            "стар" => Rank.SeniorSergeant,
            "прап" => Rank.Ensign,
            "спрап" => Rank.SeniorEnsign,
            "млейт" => Rank.JuniorLieutenant,
            "лейт" => Rank.Lieutenant,
            "слейт" => Rank.SeniorLieutenant,
            "кап" => Rank.Captain,
            "май" => Rank.Major,
            "пполк" => Rank.LieutenantColonel,
            "полк" => Rank.Colonel,
            _ => Rank.Unknown,
        };
    }

    public static Group StringToGroup(string groupString)
    {
        return groupString.ToLower() switch
        {
            "1" => Group.First,
            "2" => Group.Second,
            "3" => Group.Third,
            "4" => Group.Fourth,
            "5" => Group.Fifth,
            "6" => Group.Sixth,
            "гто" => Group.TransportSupport,
            "гоп" => Group.FireSupport,
            "гср" => Group.Recon,
            "гу" => Group.Leadership,
            _ => Group.Unknown,
        };
    }

    public static Role? StringToRole(string roleString)
    {
        return roleString.ToLower() switch
        {
            "кмд" => Role.Commander,
            "зкмд" => Role.DeputyCommander,
            "инстр" => Role.Instructor,
            "то" => Role.Creative,
            "тех" => Role.Technical,
            "ск" => Role.HumanResources,
            "основ" => Role.Founder,
            _ => null,
        };
    }

    public static Status StringToStatus(string statusString)
    {
        return statusString.ToLower() switch
        {
            "+" => Status.Active,
            "~" => Status.Vacation,
            "-" => Status.Reserve,
            "=" => Status.Left,
            _ => Status.Unknown,
        };
    }
    
    public static string RankToString(Rank rank)
    {
        return rank switch
        {
            Rank.Rookie => "Новобранец",
            Rank.Private => "Рядовой",
            Rank.Corporal => "Ефрейтор",
            Rank.JuniorSergeant => "Мл. сержант",
            Rank.Sergeant => "Сержант",
            Rank.SeniorSergeant => "Ст. сержант",
            Rank.Ensign => "Прапорщик",
            Rank.SeniorEnsign => "Ст. прапорщик",
            Rank.JuniorLieutenant => "Мл. лейтенант",
            Rank.Lieutenant => "Лейтенант",
            Rank.SeniorLieutenant => "Ст. лейтенант",
            Rank.Captain => "Капитан",
            Rank.Major => "Майор",
            Rank.LieutenantColonel => "Подполковник",
            Rank.Colonel => "Полковник",
            _ => "Неизвестно",
        };
    }

    public static string GroupToString(Group group)
    {
        return group switch
        {
            Group.First => "Гранит",
            Group.Second => "Топаз",
            Group.Third => "Агат",
            Group.Fourth => "Янтарь",
            Group.Fifth => "БГ 5",
            Group.Sixth => "БГ 6",
            Group.TransportSupport => "Заря",
            Group.FireSupport => "Тандем",
            Group.Recon => "Линза",
            Group.Leadership => "Полынь",
            _ => "неизвестно",
        };
    }

    public static string RoleToString(Role role)
    {
        return role switch
        {
            Role.Commander => "Командир",
            Role.DeputyCommander => "Зам. командир",
            Role.Instructor => "Инструктор",
            Role.Creative => "Творческий отдел",
            Role.Technical => "Технический отдел",
            Role.HumanResources => "Служба кадров",
            Role.Founder => "Основатель отряда",
            _ => "Неизвестная роль",
        };
    }

    public static string StatusToString(Status status)
    {
        return status switch
        {
            Status.Active => "Активн",
            Status.Vacation => "В отпуске",
            Status.Reserve => "В резерве",
            Status.Left => "Покинул отряд",
            _ => "Неизвестно",
        };
    }

}