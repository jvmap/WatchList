using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Data;
using WatchList.Events;

namespace WatchList.Services
{
    public class EventRoutingService : IHostedService
    {
        private readonly IEventBus _bus;
        private readonly IEventConsumer _repo;
        private readonly IEventStore _eventStore;

        public EventRoutingService(
            IEventBus bus, 
            IUserMovieRepository repo,
            IEventStore eventStore)
        {
            this._bus = bus;
            this._repo = new EventDispatcher(repo);
            this._eventStore = eventStore;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await WarmUpAsync();
            await _bus.SubscribeAsync(_repo);
        }

        private async Task WarmUpAsync()
        {
            foreach (Event evt in await _eventStore.GetEventsAsync())
            {
                await _repo.OnNextAsync(evt);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
