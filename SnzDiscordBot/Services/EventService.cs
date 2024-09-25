using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class EventService : IEventService
{
    private readonly IBaseRepo _dbRepo;
    private readonly DiscordSocketClient _client;
    
    public EventService(IBaseRepo dbRepo, DiscordSocketClient client)
    {
        _dbRepo = dbRepo;
        _client = client;
    }
    
    public async Task<EventEntity> GetEventAsync(ulong guildId, ulong channelId, ulong messageId)
    {
        var dbSet  = _dbRepo.DbContext.Set<EventEntity>();
        
        var channel = (IMessageChannel)_client.Guilds.FirstOrDefault(g => g.Id == guildId)!.Channels.FirstOrDefault(c => c.Id == channelId)!;
        var message = await channel.GetMessageAsync(messageId);
        
        var eventEntity = await dbSet.FirstOrDefaultAsync(x => x.GuildId == guildId && x.ChannelId == channelId && x.MessageId == messageId) ?? new EventEntity()
        {
            GuildId = guildId,
            ChannelId = channelId,
            MessageId = messageId,
            DateTime = message.CreatedAt,
        };
        return eventEntity;
    }
    
    public async Task<bool> AddUpdateEventAsync(ulong guildId, ulong channelId, ulong messageId, List<ulong>? votedYes = null, List<ulong>? votedNo = null, List<ulong>? votedMaybe = null)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);
        
        eventEntity.VotedYesUserIds = votedYes ?? eventEntity.VotedYesUserIds;
        eventEntity.VotedNoUserIds = votedNo ?? eventEntity.VotedNoUserIds;
        eventEntity.VotedMaybeUserIds = votedMaybe ?? eventEntity.VotedMaybeUserIds;
        
        return await _dbRepo.AddUpdateEntityAsync(eventEntity);
    }

    public async Task<bool> VoteYesAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);

        eventEntity.VotedMaybeUserIds.Remove(userId);
        eventEntity.VotedNoUserIds.Remove(userId);
        if(!eventEntity.VotedYesUserIds.Contains(userId)) eventEntity.VotedYesUserIds.Add(userId);
        
        return await AddUpdateEventAsync(guildId, channelId, messageId, eventEntity.VotedYesUserIds, eventEntity.VotedNoUserIds, eventEntity.VotedMaybeUserIds);
    }
    
    public async Task<bool> VoteNoAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);

        eventEntity.VotedYesUserIds.Remove(userId);
        eventEntity.VotedMaybeUserIds.Remove(userId);
        if(!eventEntity.VotedNoUserIds.Contains(userId)) eventEntity.VotedNoUserIds.Add(userId);
        
        return await AddUpdateEventAsync(guildId, channelId, messageId, eventEntity.VotedYesUserIds, eventEntity.VotedNoUserIds, eventEntity.VotedMaybeUserIds);
    }
    
    public async Task<bool> VoteMaybeAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId)
    {
        var eventEntity = await GetEventAsync(guildId, channelId, messageId);

        eventEntity.VotedYesUserIds.Remove(userId);
        eventEntity.VotedNoUserIds.Remove(userId);
        if(!eventEntity.VotedMaybeUserIds.Contains(userId)) eventEntity.VotedMaybeUserIds.Add(userId);
        
        return await AddUpdateEventAsync(guildId, channelId, messageId, eventEntity.VotedYesUserIds, eventEntity.VotedNoUserIds, eventEntity.VotedMaybeUserIds);
    }
}