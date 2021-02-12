using System;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Events;

namespace WatchList.CommandHandlers
{
    internal class WantToWatchMovieCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public WantToWatchMovieCommandHandler(IEventStore eventStore, IEventBus eventBus)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
        }

        public async Task HandleCommandAsync(WantToWatchMovieCommand cmd)
        {
            var evt = new WantToWatchMovieEvent { AggregateId = cmd.MovieId };
            await _eventStore.AddEventAsync(evt);
            await _eventBus.PublishEventAsync(evt);
        }
    }
}