using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnzDiscordBot.DataBase;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services.Repo;

public class BaseRepo : IBaseRepo
{
    private readonly ILogger<BaseRepo> _logger;
    public AppDbContext DbContext { get; }
    public BaseRepo(AppDbContext appDbContext, ILogger<BaseRepo> logger)
    {
        DbContext = appDbContext;
        _logger = logger;
    }

    public async Task<RepoResult<List<TEntity>>> GetAllEntityAsync<TEntity>() where TEntity : class
    {
        try
        {
            var dbSet = DbContext.Set<TEntity>();
            var res = await dbSet.ToListAsync();
            return new ()
            {
                IsSuccess = true,
                Result = res
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                Exception = ex
            };
        }
    }

    public async Task<RepoResult<TEntity>> GetEntityByIdAsync<TEntity>(int id) where TEntity : BaseEntity
    {
        try
        {
            var dbSet = DbContext.Set<TEntity>();
            var res = await dbSet.FirstOrDefaultAsync(x => x.Id == id);
            return new()
            {
                IsSuccess = true,
                Result = res
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                Exception = ex
            };
        }
    }

    public async Task<bool> AddUpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        var dbSet = DbContext.Set<TEntity>();
        try
        {
            if (entity.Id == 0)
            {
                dbSet.Add(entity);
            }
            else
            {
                dbSet.Update(entity);
            }
            await DbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return false;
        }

    }

    public async Task<bool> DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : class
    {
        var dbSet = DbContext.Set<TEntity>();
        try
        {
            dbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return false;
        }
    }
}
