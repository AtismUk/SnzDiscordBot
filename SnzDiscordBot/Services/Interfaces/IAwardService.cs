using SnzDiscordBot.DataBase.Entities;

namespace SnzDiscordBot.Services.Interfaces;

public interface IAwardService
{
    Task<AwardEntity?> UpdateAwardAsync(ulong guildId, string descriptor, int? priority = null, string? description = null, string? name = null, string? imageUrl = null);
    
    Task<AwardEntity?> RemoveAwardAsync(ulong guildId, string descriptor);
    
    Task<AwardEntity?> GetAwardAsync(ulong guildId, string descriptor);
    
    Task<(MemberEntity?, AwardEntity?, MemberAwardEntity?)> UpdateMemberAwardAsync(ulong guildId, ulong userId, string descriptor, string? awardReason = null);
    
    Task<(MemberEntity?, AwardEntity?, MemberAwardEntity?)> RemoveMemberAwardAsync(ulong guildId, ulong userId, string descriptor);
    
    Task<(MemberEntity?, Dictionary<MemberAwardEntity, AwardEntity?>?)> GetMemberAwardsAsync(ulong guildId, ulong userId);
}