using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data
{
    public class InMemoryUserMovieRepository : IUserMovieRepository
    {
        private readonly Dictionary<string, int> _movieTimesWatched = new Dictionary<string, int>();
        private readonly HashSet<string> _wantToWatchMovies = new HashSet<string>();
        private readonly Dictionary<string, (int count, float average)> _movieRatings 
            = new Dictionary<string, (int count, float average)>();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public async Task<UserMovieData> GetUserMovieDataByIdAsync(string movieId)
        {
            var result = new UserMovieData();
            await _lock.WaitAsync();
            try
            {
                result.TimesWatched = _movieTimesWatched.GetValueOrDefault(movieId);
                result.WantToWatch = _wantToWatchMovies.Contains(movieId);
                result.Rating = _movieRatings.GetValueOrDefault(movieId).average;
            }
            finally
            {
                _lock.Release();
            }
            if (result.Rating == 0)
                result.Rating = null;
            return result;
        }

        public async Task OnNextAsync(RatedMovieEvent evt)
        {
            await _lock.WaitAsync();
            try
            {
                int newRating = evt.Rating;
                AddOrUpdate(_movieRatings, evt.AggregateId, (1, newRating), t => UpdateRating(t, newRating));
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task OnNextAsync(WantToWatchMovieEvent evt)
        {
            await _lock.WaitAsync();
            try
            {                
                _wantToWatchMovies.Add(evt.AggregateId);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task OnNextAsync(WatchedMovieEvent evt)
        {
            await _lock.WaitAsync();
            try
            {
                AddOrUpdate(_movieTimesWatched, evt.AggregateId, 1, times => times + 1);
                _wantToWatchMovies.Remove(evt.AggregateId);
            }
            finally
            {
                _lock.Release();
            }
        }

        private (int count, float average) UpdateRating((int count, float average) @in, int newRating)
        {
            float newAverage = (@in.count * @in.average + newRating) / (@in.count + 1); 
            return (@in.count + 1, newAverage);
        }

        private static void AddOrUpdate<K, V>(Dictionary<K, V> dict, K key, V addValue, Func<V, V> updateValue)
        {
            if (dict.TryGetValue(key, out V value))
            {
                dict[key] = updateValue(value);
            }
            else
            {
                dict.Add(key, addValue);
            }
        }
    }
}
