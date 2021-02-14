using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Data;
using WatchList.Domain.Commands;
using WatchList.Domain.Entities;
using WatchList.Domain.Events;
using WatchList.Events;

namespace WatchList.Commands
{
    public class CommandInvoker
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public CommandInvoker(IEventStore eventStore, IEventBus eventBus)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
        }
        
        public async Task InvokeAsync<TEntity>(Command<TEntity> cmd)
            where TEntity : Entity, new()
        {
            TEntity entity = await ReviveAsync<TEntity>(cmd.AggregateId);
            var dispatcher = new CommandDispatcher<TEntity>(entity);
            IEnumerable<Event> newEvents = dispatcher
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
            var dispatcher = new EventDispatcher(entity);
            foreach (Event evt in await _eventStore.GetEventsAsync(aggregateId))
            {
                dispatcher.OnNext(evt);
            }
            return entity;
        }
    }
}
