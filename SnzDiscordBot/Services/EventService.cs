using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class EventService : IEventService
{
    private readonly IBaseRepo _dbRepo;
    
    public EventService(IBaseRepo dbRepo)
    {
        _dbRepo = dbRepo;
    }
    
    public async Task<EventEntity?> GetEventAsync(ulong guildId, ulong channelId, ulong messageId)
    {
        var dbSet  = _dbRepo.DbContext.Set<EventEntity>();
        var eventEntity = await dbSet.FirstOrDefaultAsync(x => x.GuildId == guildId && x.ChannelId == channelId && x.MessageId == messageId);
        return eventEntity;
    }
    
    public async Task<bool> AddUpdateEventAsync(ulong guildId, ulong channelId, ulong messageId, List<ulong>? votedYes = null, List<ulong>? votedNo = null, List<ulong>? votedMaybe = null)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId) ?? new EventEntity()
        {
            GuildId = guildId,
            ChannelId = channelId,
            MessageId = messageId
        };
        
        eventEntity.VotedYesUserIds = votedYes ?? eventEntity.VotedYesUserIds;
        eventEntity.VotedNoUserIds = votedYes ?? eventEntity.VotedNoUserIds;
        eventEntity.VotedMaybeUserIds = votedYes ?? eventEntity.VotedMaybeUserIds;
        
        return await _dbRepo.AddUpdateEntityAsync(eventEntity);
    }

    public async Task<bool> VoteYesAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) 
            return false;

        if (eventEntity.VotedMaybeUserIds.Contains(userId)) eventEntity.VotedMaybeUserIds.Remove(userId);
        if (eventEntity.VotedNoUserIds.Contains(userId)) eventEntity.VotedNoUserIds.Remove(userId);
        eventEntity.VotedYesUserIds.Add(userId);
        
        return await AddUpdateEventAsync(guildId, channelId, messageId, eventEntity.VotedNoUserIds, eventEntity.VotedYesUserIds, eventEntity.VotedMaybeUserIds);
    }
    
    public async Task<bool> VoteNoAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) 
            return false;
        
        if (eventEntity.VotedYesUserIds.Contains(userId)) eventEntity.VotedMaybeUserIds.Remove(userId);
        if (eventEntity.VotedMaybeUserIds.Contains(userId)) eventEntity.VotedNoUserIds.Remove(userId);
        eventEntity.VotedNoUserIds.Add(userId);
        
        return await AddUpdateEventAsync(guildId, channelId, messageId, eventEntity.VotedNoUserIds, eventEntity.VotedYesUserIds, eventEntity.VotedMaybeUserIds);
    }
    
    public async Task<bool> VoteMaybeAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        if (eventEntity == null) 
            return false;
        
        if (eventEntity.VotedYesUserIds.Contains(userId)) eventEntity.VotedMaybeUserIds.Remove(userId);
        if (eventEntity.VotedNoUserIds.Contains(userId)) eventEntity.VotedNoUserIds.Remove(userId);
        eventEntity.VotedMaybeUserIds.Add(userId);
        
        return await AddUpdateEventAsync(guildId, channelId, messageId, eventEntity.VotedNoUserIds, eventEntity.VotedYesUserIds, eventEntity.VotedMaybeUserIds);
    }
}