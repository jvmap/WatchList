using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Domain;
using WatchList.Events;

namespace WatchList.CommandHandlers
{
    public class RateMovieCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;
        private readonly IUserMovieRepository _userMovieRepository;

        public RateMovieCommandHandler(
            IEventStore eventStore, 
            IEventBus eventBus,
            IUserMovieRepository userMovieRepository)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
            this._userMovieRepository = userMovieRepository;
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
            var processor = new EventProcessor();
            foreach (Event evt in await _eventStore.GetEventsAsync(movieId))
            {
                processor.OnNext(evt);
            }
            return processor.Movie;
        }

        private class EventProcessor
        {
            public Movie Movie { get; private set; }

            public EventProcessor()
            {
                Movie = new Movie();
            }

            public void OnNext(Event evt)
            {
                switch (evt.Name)
                {
                    case "WatchedMovie":
                        Movie.Watched();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
