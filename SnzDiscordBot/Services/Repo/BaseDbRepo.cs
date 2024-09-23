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
    public class BaseDbRepo : IBaseDbRepo
    {
        readonly AppDbContext _dbContext;
        public BaseDbRepo(AppDbContext appDbContext)
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
    }
}
