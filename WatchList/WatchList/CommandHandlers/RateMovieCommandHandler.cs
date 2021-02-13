using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Domain.Entities;
using WatchList.Domain.Events;
using WatchList.Events;

namespace WatchList.CommandHandlers
{
    public class RateMovieCommandHandler : ICommandHandler<RateMovieCommand, Movie>
    {
        public IEnumerable<Event> Handle(Movie movie, RateMovieCommand cmd)
        {
            if (movie.TimesWatched <= 0)
                throw new InvalidOperationException("Please watch movie before rating it.");

            yield return new RatedMovieEvent { AggregateId = cmd.AggregateId, Rating = cmd.Rating };
        }
    }
}
