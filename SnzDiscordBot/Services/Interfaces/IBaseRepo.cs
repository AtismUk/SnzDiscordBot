using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Repo;
using SnzDiscordBot.DataBase;

namespace SnzDiscordBot.Services.Interfaces;

public interface IBaseRepo
{
    AppDbContext DbContext { get; }
    
    Task<RepoResult<List<TEntity>>> GetAllEntityAsync<TEntity>() where TEntity : class;

    Task<RepoResult<TEntity>> GetEntityByIdAsync<TEntity>(int id) where TEntity : BaseEntity;
    
    Task<bool> AddUpdateEntityAsync<TEntity>(TEntity newEntity) where TEntity : BaseEntity;

    Task<bool> DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : class;
}
