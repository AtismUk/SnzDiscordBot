using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnzDiscordBot.Services.Interfaces
{
    public interface IBaseRepo
    {
        RepoResult<List<TEntity>> GetAllEntity<TEntity>() where TEntity : class;

        RepoResult<TEntity> GetEntityById<TEntity>(int id) where TEntity : BaseEntity;
    }
}
