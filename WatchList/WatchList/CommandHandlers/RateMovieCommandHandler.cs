using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
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
            UserMovieData movieData = await _userMovieRepository.GetUserMovieDataByIdAsync(cmd.MovieId);
            if (movieData.TimesWatched > 0)
            {
                var evt = new RatedMovieEvent(cmd.MovieId, cmd.Rating);
                await _eventStore.AddEventAsync(evt);
                await _eventBus.PublishEventAsync(evt);
            }
            else
                throw new InvalidOperationException("Please watch movie before rating it.");
        }
    }
}
