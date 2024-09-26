using System.Linq.Expressions;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.DataBase;

namespace SnzDiscordBot.Services.Interfaces;

public interface IBaseRepo
{
    AppDbContext DbContext { get; }

    Task<List<TEntity>?> GetAllEntityAsync<TEntity>(Func<TEntity, bool>? predicate = null) where TEntity : class;

    Task<TEntity?> GetEntityByIdAsync<TEntity>(int id) where TEntity : BaseEntity;
    
    Task<TEntity?> AddUpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

    Task<TEntity?> DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : class;

    Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null) where TEntity : class;
}
