using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Events;

namespace WatchList.Data
{
    public class InMemoryUserMovieRepository : IUserMovieRepository
    {
        private readonly HashSet<string> _watchedMovies = new HashSet<string>();
        
        public Task<UserMovieData> GetUserMovieDataByIdAsync(string movieId)
        {
            var result = new UserMovieData();
            result.Watched = _watchedMovies.Contains(movieId);
            return Task.FromResult(result);
        }

        public void OnNext(IEvent evt)
        {
            switch (evt.Name)
            {
                case "WatchedMovie":
                    _watchedMovies.Add(evt.AggregateId);
                    break;
                default:
                    break;
            }
        }
    }
}
