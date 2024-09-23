using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnzDiscordBot.Services.Repo
{
    public class RepoResult<T>
    {
        public bool IsSuccess { get; set; } = false;
        public T Result { get; set; }
        public Exception? Exception { get; set; }
    }
}
