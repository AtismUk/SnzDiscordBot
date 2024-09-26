using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class AwardService : IAwardService
{
    private readonly IBaseRepo _baseRepo;
    private readonly IMemberService _memberService;

    public AwardService(IMemberService memberService, IBaseRepo baseRepo)
    {
        _memberService = memberService;
        _baseRepo = baseRepo;
    }

    public async Task<AwardEntity?> UpdateAwardAsync(ulong guildId, string descriptor, int? priority = null, string? description = null, string? name = null, string? imageUrl = null)
    {
        // Пытаемся найти существующую запись для изменения или создаем новую.
        var award = await GetAwardAsync(guildId, descriptor) ?? new AwardEntity(guildId, descriptor);

        // Обновляем данные
        award.Priority = priority ?? award.Priority;
        award.Name = name ?? award.Name;
        award.Description = description ?? award.Description;
        award.ImageUrl = imageUrl ?? award.ImageUrl;

        // Передаем в BaseRepo
        return await _baseRepo.AddUpdateEntityAsync(award);
    }

    public async Task<AwardEntity?> RemoveAwardAsync(ulong guildId, string descriptor)
    {
        // Пытаемся найти существующую запись для удаления
        var award = await GetAwardAsync(guildId, descriptor);
        if (award == null) return null; // Если записи не существует, то возвращаем null
        
        // Передаем в BaseRepo
        return await  _baseRepo.DeleteEntityAsync(award);
    }

    public async Task<AwardEntity?> GetAwardAsync(ulong guildId, string descriptor)
    {
        // Пытаемся найти первую подходящую запись.
        return await _baseRepo.FirstOrDefaultAsync<AwardEntity>(x => x.GuildId == guildId && x.Descriptor == descriptor);
    }
    

    public async Task<(MemberEntity?, AwardEntity?, MemberAwardEntity?)> UpdateMemberAwardAsync(ulong guildId, ulong userId, string descriptor, string? awardReason = null)
    {
        // Ищем запись целевого пользователя
        var member = await _memberService.GetMemberAsync(guildId, userId);
        if (member == null) return (null, null, null);

        // Ищем запись целевой награды
        var award = await GetAwardAsync(guildId, descriptor);
        if (award == null) return (member, null, null);

        // Ищем целевое награждение
        var memberAward = await _baseRepo.FirstOrDefaultAsync<MemberAwardEntity>(x => x.UserId == userId && x.AwardDescriptor == descriptor) ?? new MemberAwardEntity(descriptor, userId, guildId, DateTime.Now);
        
        // Обновляем данные
        memberAward.AwardReason = awardReason ?? memberAward.AwardReason;
        
        // Передаем в BaseRepo
        return (member, award, await _baseRepo.AddUpdateEntityAsync(memberAward));
    }

    public async Task<(MemberEntity?, AwardEntity?, MemberAwardEntity?)> RemoveMemberAwardAsync(ulong guildId, ulong userId, string descriptor)
    {
        // Ищем запись целевого пользователя
        var member = await _memberService.GetMemberAsync(guildId, userId);
        if (member == null) return (null, null, null); // Если записи не существует, то возвращаем null-ы
        
        // Ищем запись целевой награды
        var award = await GetAwardAsync(guildId, descriptor);
        if (award == null) return (member, null, null); // Если записи не существует, то возвращаем пользователя и null-ы
        
        // Ищем целевое награждение
        var memberAward = await _baseRepo.FirstOrDefaultAsync<MemberAwardEntity>(x => x.UserId == userId);
        if (memberAward == null) return (member, award, null); // Если записи не существует, то возвращаем пользователя, награду и null.
        
        // Передаем в BaseRepo
        return (member, award, await _baseRepo.DeleteEntityAsync(memberAward));
    }

    public async Task<(MemberEntity?, Dictionary<MemberAwardEntity, AwardEntity?>?)> GetMemberAwardsAsync(ulong guildId, ulong userId)
    {
        // Ищем запись целевого пользователя
        var member = await _memberService.GetMemberAsync(guildId, userId);
        if (member == null) return (null, null);
        
        // Ищем записи награждений
        var memberAwards = await _baseRepo.GetAllEntityAsync<MemberAwardEntity>(x => x.UserId == userId && x.GuildId == guildId);
        if (memberAwards == null) return (member, null);
        
        // Создаем пустой словарь
        var awardsDict = new Dictionary<MemberAwardEntity, AwardEntity?>();

        // Проходимся по записям награждений для заполнения словаря
        foreach (var memberAward in memberAwards)
        {
            // Ищем награду
            var award = await GetAwardAsync(memberAward.GuildId, memberAward.AwardDescriptor);
            // Заполняем словарь
            awardsDict[memberAward] = award;
        }

        return (member, awardsDict);
    }
}