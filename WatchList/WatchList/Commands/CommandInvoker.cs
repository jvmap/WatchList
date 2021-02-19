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
        private readonly DynamicDispatcher _dispatcher;

        public CommandInvoker(IEventStore eventStore, DynamicDispatcher dispatcher)
        {
            this._eventStore = eventStore;
            this._dispatcher = dispatcher;
        }
        
        public async Task InvokeAsync<TEntity>(Command<TEntity> cmd)
            where TEntity : Entity, new()
        {
            (TEntity entity, ConcurrencyToken token) = await ReviveAsync<TEntity>(cmd.AggregateId);
            var commandDispatcher = new CommandDispatcher<TEntity>(entity, _dispatcher);
            List<Event> newEvents = commandDispatcher
                .Handle(cmd)
                .ToList();
            await _eventStore.AddEventsAsync(newEvents, token);
        }

        private async Task<(TEntity, ConcurrencyToken)> ReviveAsync<TEntity>(string aggregateId)
            where TEntity : Entity, new()
        {
            var entity = new TEntity() { AggregateId = aggregateId };
            var eventDispatcher = new EventDispatcher(entity, _dispatcher);
            (ICollection<Event> evts, ConcurrencyToken token) = await _eventStore.GetEventsAsync(aggregateId);
            foreach (Event evt in evts)
            {
                eventDispatcher.OnNext(evt);
            }
            return (entity, token);
        }
    }
}
