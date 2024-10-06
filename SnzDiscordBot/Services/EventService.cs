using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class EventService : IEventService
{
    private readonly IBaseRepo _baseRepo;
    private readonly IMemberService _memberService;

    public EventService(IBaseRepo baseRepo, IMemberService memberService)
    {
        _memberService = memberService;
        _baseRepo = baseRepo;
    }

    public async Task<EventEntity?> GetEventAsync(ulong guildId, ulong channelId, ulong messageId)
    {
        // Пытаемся найти первую подходящую запись.
        return await _baseRepo.FirstOrDefaultAsync<EventEntity>(s => s.GuildId == guildId && s.ChannelId == channelId && s.MessageId == messageId);
    }

    public async Task<EventEntity?> AddUpdateEventAsync(ulong guildId, ulong channelId, ulong messageId, DateTime? startAt = null)
    {
        // Пытаемся найти существующую запись для изменения или создаем новую.
        var eventEntity = await GetEventAsync(guildId, channelId, messageId) ?? new EventEntity(guildId, channelId, messageId);

        // Обновляем данные
        eventEntity.StartAt = startAt ?? eventEntity.StartAt;
        
        // Передаем в BaseRepo
        return await _baseRepo.AddUpdateEntityAsync(eventEntity);
    }

    public async Task<EventEntity?> RemoveEventAsync(ulong guildId, ulong channelId, ulong messageId)
    {
        // Пытаемся найти существующую запись для удаления
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) return null; // Если записи не существует, то возвращаем null

        // Передаем в BaseRepo
        return await _baseRepo.DeleteEntityAsync(eventEntity);
    }

    public async Task<(EventEntity?, MemberEntity?, EventVoteEntity?)> GetVoteAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        // Ищем запись целевого события
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) return (null, null, null); // Если записи не существует, то возвращаем null-ы
        
        // Ищем запись целевого пользователя
        var member = await _memberService.GetMemberAsync(guildId, userId);
        if (member == null) return (eventEntity, null, null); // Если записи не существует, то возвращаем событие и null-ы
        
        // Ищем целевой голос
        var vote = await _baseRepo.FirstOrDefaultAsync<EventVoteEntity>(x => x.GuildId == guildId && x.UserId == member.UserId && x.EventMessageId == eventEntity.MessageId);
        
        return (eventEntity, member, vote);
    }
    
    public async Task<(EventEntity?, MemberEntity?, EventVoteEntity?)> AddUpdateVoteAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId, VoteType voteType)
    {
        // Ищем запись целевого события
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) return (null, null, null);
        
        // Ищем запись целевого пользователя
        var member = await _memberService.GetMemberAsync(guildId, userId);
        if (member == null) return (eventEntity, null, null);
        
        // Ищем целевой голос
        var vote = await _baseRepo.FirstOrDefaultAsync<EventVoteEntity>(x => x.GuildId == guildId && x.UserId == member.UserId && x.EventMessageId == eventEntity.MessageId) ?? new EventVoteEntity(guildId, channelId, messageId);

        // Обновляем данные
        vote.Type = voteType;
        
        return (eventEntity, member, await _baseRepo.AddUpdateEntityAsync(vote)); 
    }

    public async Task<(EventEntity?, Dictionary<EventVoteEntity, MemberEntity?>?)> GetAllVotesAsync(ulong guildId, ulong channelId, ulong messageId)
    {
        // Ищем запись целевого события
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) return (null, null); // Если записи не существует, то возвращаем null-ы

        // Ищем записи голосов по целевому событию
        var votes = await _baseRepo.GetAllEntityAsync<EventVoteEntity>(x => x.GuildId == guildId && x.EventMessageId == eventEntity.MessageId);
        if (votes == null) return (eventEntity, null); // Если записей не существует, то возвращаем событие и null-ы

        // Создаем пустой словарь
        var votesDictionary = new Dictionary<EventVoteEntity, MemberEntity?>();

        // Проходимся по записям голосов для заполнения словаря
        foreach (var vote in votes)
        {
            // Ищем голосовавшего пользователя
            var member = await _memberService.GetMemberAsync(guildId, vote.UserId);
            // Заполняем словарь
            votesDictionary.Add(vote, member);
        }

        return (eventEntity, votesDictionary);
    }
}