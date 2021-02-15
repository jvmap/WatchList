using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Data;
using WatchList.Domain.Commands;
using WatchList.Domain.Entities;
using WatchList.Domain.Events;
using WatchList.DynamicDispatch;
using WatchList.Events;

namespace WatchList.Commands
{
    public class CommandInvoker
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;
        private readonly DynamicDispatcher _dispatcher;

        public CommandInvoker(IEventStore eventStore, IEventBus eventBus, DynamicDispatcher dispatcher)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
            this._dispatcher = dispatcher;
        }
        
        public async Task InvokeAsync<TEntity>(Command<TEntity> cmd)
            where TEntity : Entity, new()
        {
            TEntity entity = await ReviveAsync<TEntity>(cmd.AggregateId);
            var commandDispatcher = new CommandDispatcher<TEntity>(entity, _dispatcher);
            IEnumerable<Event> newEvents = commandDispatcher
                .Handle(cmd)
                .ToList();
            await _eventStore.AddEventsAsync(newEvents);
            foreach (Event @event in newEvents)
            {
                await _eventBus.PublishEventAsync(@event);
            }
        }

        private async Task<TEntity> ReviveAsync<TEntity>(string aggregateId)
            where TEntity : Entity, new()
        {
            var entity = new TEntity() { AggregateId = aggregateId };
            var eventDispatcher = new EventDispatcher(entity, _dispatcher);
            foreach (Event evt in await _eventStore.GetEventsAsync(aggregateId))
            {
                eventDispatcher.OnNext(evt);
            }
            return entity;
        }
    }
}
