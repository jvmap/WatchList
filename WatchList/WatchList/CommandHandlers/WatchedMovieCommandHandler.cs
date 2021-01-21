using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Data;
using WatchList.Events;

namespace WatchList.CommandHandlers
{
    public class WatchedMovieCommandHandler
    {
        private readonly IEventStore _store;
        private readonly IEventBus _bus;

        public WatchedMovieCommandHandler(IEventStore store, IEventBus bus)
        {
            this._store = store;
            this._bus = bus;
        }
        
        public async Task HandleCommandAsync(WatchedMovieCommand cmd)
        {
            var evt = new WatchedMovieEvent(cmd.MovieId);
            await _store.AddEventAsync(evt);
            await _bus.PublishEventAsync(evt);
        }
    }
}
