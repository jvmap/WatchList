using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Data;
using WatchList.Domain.Events;
using WatchList.DynamicDispatch;
using WatchList.Events;

namespace WatchList.Services
{
    public class EventRoutingService : IHostedService
    {
        private readonly IEventConsumer _repo;
        private readonly IEventStore _eventStore;

        public EventRoutingService(
            IEventStore eventStore, 
            IUserMovieRepository repo,
            DynamicDispatcher dispatcher)
        {
            this._eventStore = eventStore;
            this._repo = new _EventDispatcherWrapper(new EventDispatcher(repo, dispatcher));
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventStore.SubscribeAsync(_repo);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        class _EventDispatcherWrapper : IEventConsumer
        {
            private EventDispatcher _eventDispatcher;

            public _EventDispatcherWrapper(EventDispatcher eventDispatcher)
            {
                this._eventDispatcher = eventDispatcher;
            }

            public async Task ProcessBatchAsync(IReadOnlyCollection<(long index, Event)> batch)
            {
                foreach ((long index, Event evt) in batch)
                {
                    await _eventDispatcher.OnNextAsync(evt);
                }
            }
        }
    }
}
