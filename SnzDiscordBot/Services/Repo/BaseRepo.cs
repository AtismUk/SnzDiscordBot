using Discord;
using SnzDiscordBot.DataBase;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnzDiscordBot.Services.Repo
{
    public class BaseRepo : IBaseRepo
    {
        readonly AppDbContext _dbContext;
        public BaseRepo(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public RepoResult<List<TEntity>> GetAllEntity<TEntity>() where TEntity : class
        {
            try
            {
                var dbSet = _dbContext.Set<TEntity>();
                var res = dbSet.ToList();
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

        public RepoResult<TEntity> GetEntityById<TEntity>(int id) where TEntity : BaseEntity
        {
            try
            {
                var dbSet = _dbContext.Set<TEntity>();
                var res = dbSet.FirstOrDefault(x => x.Id == id);
                return new()
                {
                    IsSuccess = true,
                    Result = res!
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

        public bool AddUpdateEntity<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var dbSet = _dbContext.Set<TEntity>();
            if (entity.Id == 0)
            {
                dbSet.Add(entity);
            }
            else
            {
                dbSet.Update(entity);
            }
            try
            {
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = _dbContext.Set<TEntity>();
            try
            {
                dbSet.Remove(entity);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
