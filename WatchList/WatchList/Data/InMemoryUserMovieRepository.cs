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
        private readonly HashSet<string> _wantToWatchMovies = new HashSet<string>();
        private readonly Dictionary<string, int> _movieRatings = new Dictionary<string, int>();

        public Task<UserMovieData> GetUserMovieDataByIdAsync(string movieId)
        {
            var result = new UserMovieData();
            result.Watched = _watchedMovies.Contains(movieId);
            result.WantToWatch = _wantToWatchMovies.Contains(movieId);
            result.Rating = _movieRatings.GetValueOrDefault(movieId);
            if (result.Rating == 0)
                result.Rating = null;
            return Task.FromResult(result);
        }

        public void OnNext(IEvent evt)
        {
            switch (evt.Name)
            {
                case "WatchedMovie":
                    _watchedMovies.Add(evt.AggregateId);
                    _wantToWatchMovies.Remove(evt.AggregateId);
                    break;
                case "WantToWatchMovie":
                    _wantToWatchMovies.Add(evt.AggregateId);
                    break;
                case "RatedMovie":
                    _movieRatings[evt.AggregateId] = int.Parse(GetProperty(evt.EventData, "rating"));
                    break;
                default:
                    break;
            }
        }

        private static string GetProperty(IEnumerable<(string, string)> keyValuePairs, string key)
        {
            return keyValuePairs
                .First(kvp => kvp.Item1 == key)
                .Item2;
        }
    }
}
