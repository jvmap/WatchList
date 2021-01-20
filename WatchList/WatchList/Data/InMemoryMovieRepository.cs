using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Data
{
    public class InMemoryMovieRepository : IMovieRepository
    {
        private static readonly IReadOnlyDictionary<string, MovieData> _movies;
        
        public Task<MovieData> GetMovieByTitleAsync(string title)
        {
            return Task.FromResult(GetMovieByTitle(title));
        }

        private static MovieData GetMovieByTitle(string title)
        {
            if (_movies.TryGetValue(title, out MovieData value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        static InMemoryMovieRepository()
        {
            var movies = new Dictionary<string, MovieData>(StringComparer.InvariantCultureIgnoreCase);
            _movies = movies;

            movies.Add("Saving Private Ryan", new MovieData(new[] { ("Title", "Saving Private Ryan"), ("Year", "1991") }));
        }
    }
}
