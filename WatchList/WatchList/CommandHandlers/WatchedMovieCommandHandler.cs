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
    public class WatchedMovieCommandHandler : ICommandHandler<WatchedMovieCommand, Movie>
    {
        public IEnumerable<Event> Handle(Movie movie, WatchedMovieCommand cmd)
        {
            yield return new WatchedMovieEvent { AggregateId = cmd.AggregateId };
        }
    }
}
