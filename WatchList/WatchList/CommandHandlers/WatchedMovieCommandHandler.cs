using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Domain.Events;
using WatchList.Events;

namespace WatchList.CommandHandlers
{
    public class WatchedMovieCommandHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public WatchedMovieCommandHandler(IEventStore eventStore, IEventBus eventBus)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
        }
        
        public async Task HandleCommandAsync(WatchedMovieCommand cmd)
        {
            var evt = new WatchedMovieEvent { AggregateId = cmd.MovieId };
            await _eventStore.AddEventAsync(evt);
            await _eventBus.PublishEventAsync(evt);
        }
    }
}
