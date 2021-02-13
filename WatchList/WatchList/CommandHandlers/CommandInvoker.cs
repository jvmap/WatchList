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
    public class CommandInvoker
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public CommandInvoker(IEventStore eventStore, IEventBus eventBus)
        {
            this._eventStore = eventStore;
            this._eventBus = eventBus;
        }
        
        public async Task InvokeAsync<TEntity, TCommand>(
            TCommand cmd, 
            ICommandHandler<TCommand, TEntity> handler
            )
            where TCommand : Command
            where TEntity : new()
        {
            TEntity entity = await ReviveAsync<TEntity>(cmd.AggregateId);
            IEnumerable<Event> newEvents = handler
                .Handle(entity, cmd)
                .ToList();
            await _eventStore.AddEventsAsync(newEvents);
            foreach (Event @event in newEvents)
            {
                await _eventBus.PublishEventAsync(@event);
            }
        }

        private async Task<TEntity> ReviveAsync<TEntity>(string aggregateId)
            where TEntity : new()
        {
            var entity = new TEntity();
            var dispatcher = new EventDispatcher(entity);
            foreach (Event evt in await _eventStore.GetEventsAsync(aggregateId))
            {
                dispatcher.OnNext(evt);
            }
            return entity;
        }
    }
}
