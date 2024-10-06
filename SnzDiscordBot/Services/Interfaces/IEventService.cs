using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IEventService
{
    Task<EventEntity?> GetEventAsync(ulong guildId, ulong channelId, ulong messageId);

    Task<EventEntity?> AddUpdateEventAsync(ulong guildId, ulong channelId, ulong messageId, 
        DateTime? startAt = null);

    Task<EventEntity?> RemoveEventAsync(ulong guildId, ulong channelId, ulong messageId);

    Task<(EventEntity?, MemberEntity?, EventVoteEntity?)> AddUpdateVoteAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId, 
        VoteType voteType);
    
    Task<(EventEntity?, MemberEntity?, EventVoteEntity?)> GetVoteAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId);
    
    Task<(EventEntity?, Dictionary<EventVoteEntity, MemberEntity?>?)> GetAllVotesAsync(ulong guildId, ulong channelId, ulong messageId);
}