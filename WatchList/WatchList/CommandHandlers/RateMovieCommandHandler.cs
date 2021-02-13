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
    public class RateMovieCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public RateMovieCommandHandler(
            IEventStore eventStore, 
            IEventBus eventBus)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
        }

        public async Task HandleCommandAsync(RateMovieCommand cmd)
        {
            Movie movie = await ReviveMovieAsync(cmd.MovieId);
            if (movie.TimesWatched > 0)
            {
                var evt = new RatedMovieEvent { AggregateId = cmd.MovieId, Rating = cmd.Rating };
                await _eventStore.AddEventAsync(evt);
                await _eventBus.PublishEventAsync(evt);
            }
            else
                throw new InvalidOperationException("Please watch movie before rating it.");
        }

        private async Task<Movie> ReviveMovieAsync(string movieId)
        {
            var movie = new Movie();
            var dispatcher = new EventDispatcher(movie);
            foreach (Event evt in await _eventStore.GetEventsAsync(movieId))
            {
                dispatcher.OnNext(evt);
            }
            return movie;
        }
    }
}
