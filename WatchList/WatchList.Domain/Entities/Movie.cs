using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Commands;
using WatchList.Domain.Events;

namespace WatchList.Domain.Entities
{
    public class Movie : Entity
    {
        private int _timesWatched;

        public void OnNext(WatchedMovieEvent evt)
        {
            _timesWatched++;
        }

        public IEnumerable<Event> Handle(WatchedMovieCommand cmd)
        {
            yield return new WatchedMovieEvent { AggregateId = AggregateId };
        }

        public IEnumerable<Event> Handle(WantToWatchMovieCommand cmd)
        {
            yield return new WantToWatchMovieEvent { AggregateId = AggregateId };
        }

        public IEnumerable<Event> Handle(RateMovieCommand cmd)
        {
            if (_timesWatched <= 0)
                throw new InvalidOperationException($"Please watch movie {AggregateId} before rating it.");

            yield return new RatedMovieEvent { AggregateId = AggregateId, Rating = cmd.Rating };
        }
    }
}
