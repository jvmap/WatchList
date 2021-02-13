using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Domain.Entities;
using WatchList.Domain.Events;
using WatchList.Events;

namespace WatchList.CommandHandlers
{
    internal class WantToWatchMovieCommandHandler : ICommandHandler<WantToWatchMovieCommand, Movie>
    {
        public IEnumerable<Event> Handle(Movie movie, WantToWatchMovieCommand cmd)
        {
            yield return new WantToWatchMovieEvent { AggregateId = cmd.AggregateId };
        }
    }
}