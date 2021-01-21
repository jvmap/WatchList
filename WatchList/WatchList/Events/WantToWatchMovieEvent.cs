using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public class WantToWatchMovieEvent : Event
    {
        private readonly string _movieId;

        public WantToWatchMovieEvent(string movieId)
        {
            this._movieId = movieId;
        }

        public override string AggregateId => _movieId;

        public override IEnumerable<(string, string)> EventData
            => Enumerable.Empty<(string, string)>();
    }
}
