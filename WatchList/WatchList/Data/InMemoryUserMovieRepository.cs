using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data
{
    public class InMemoryUserMovieRepository : IUserMovieRepository
    {
        public Task<UserMovieData> GetUserMovieDataByIdAsync(string movieId)
        {
            throw new NotImplementedException();
        }
    }
}
