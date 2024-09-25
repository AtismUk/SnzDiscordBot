using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IEventService
{
    Task<EventEntity?> GetEventAsync(ulong guildId, ulong channelId, ulong messageId);

    Task<bool> AddUpdateEventAsync(ulong guildId, ulong channelId, ulong messageId, 
        List<ulong>? votedYes = null,
        List<ulong>? votedNo = null, 
        List<ulong>? votedMaybe = null);

    Task<bool> VoteYesAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId);

    Task<bool> VoteNoAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId);

    Task<bool> VoteMaybeAsync(ulong guildId, ulong channelId, ulong messageId, ulong userId);
}