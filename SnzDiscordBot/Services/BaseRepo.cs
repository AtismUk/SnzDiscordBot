using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnzDiscordBot.DataBase;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Services;

public class BaseRepo : IBaseRepo
{
    private readonly ILogger<BaseRepo> _logger;
    public AppDbContext DbContext { get; }
    public BaseRepo(AppDbContext appDbContext, ILogger<BaseRepo> logger)
    {
        DbContext = appDbContext;
        _logger = logger;
    }
    
    public async Task<List<TEntity>?> GetAllEntityAsync<TEntity>(Func<TEntity, bool>? predicate = null) where TEntity : class
    {
        try
        {
            // Берем DbSet
            var dbSet = DbContext.Set<TEntity>();
            
            // Преобразуем его в лист
            var res = await dbSet.ToListAsync();

            if (predicate != null)
            {
                // Если есть выражение, то обрабаотываем его
                res = await res.ToAsyncEnumerable().Where(predicate).ToListAsync();
            }
            
            return res;
        }
        catch (Exception ex)
        {
            // Логгируем если ошибка
            _logger.LogError(ex.ToString());
            return null;
        }
    }
    
    // SafeFirstOrDefaultAsync
    public async Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null) where TEntity : class
    {
        try
        {
            // Берем DbSet
            var dbSet = DbContext.Set<TEntity>();

            // Объявляем переменную
            TEntity? res;
            
            if (predicate != null) // Если есть выражение, то обрабатываем его и/или берем первую запись
                res = await dbSet.FirstOrDefaultAsync(predicate);
            else
                res = await dbSet.FirstOrDefaultAsync();
            
            return res;
        }
        catch (Exception ex)
        {
            // Логгируем если ошибка
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task<TEntity?> GetEntityByIdAsync<TEntity>(int id) where TEntity : BaseEntity
    {
        try
        {
            // Ищем первую подходящую запись
            return await FirstOrDefaultAsync<TEntity>(x => x.Id == id);
        }
        catch (Exception ex)
        {
            // Логгируем если ошибка
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public async Task<TEntity?> AddUpdateEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        try
        {
            // Берем DbSet
            var dbSet = DbContext.Set<TEntity>();

            // Добавляем или обновляем запись
            var entityEntry = entity.Id == 0 ? dbSet.Add(entity) : dbSet.Update(entity);
            
            // Сохраняем изменения
            await DbContext.SaveChangesAsync();
            
            // Возвращаем измененную запись
            return entityEntry.Entity;
        }
        catch (Exception ex)
        {
            // Логгируем если ошибка
            _logger.LogError(ex.ToString());
            return null;
        }

    }

    public async Task<TEntity?> DeleteEntityAsync<TEntity>(TEntity entity) where TEntity : class
    {
        try
        {
            // Берем DbSet
            var dbSet = DbContext.Set<TEntity>();
            
            // Удаляем запись
            var deletedEntity = dbSet.Remove(entity);
            
            // Сохраняем изменения
            await DbContext.SaveChangesAsync();
            
            // Возвращаем удаленную запись
            return deletedEntity.Entity;
        }
        catch (Exception ex)
        {
            // Логгируем если ошибка
            _logger.LogError(ex.ToString());
            return null;
        }
    }
}
