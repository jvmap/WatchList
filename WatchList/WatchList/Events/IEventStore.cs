using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Events
{
    public interface IEventStore
    {
        long MinIndex { get; }

        Task<ICollection<Event>> GetEventsAsync()
        {
            return GetEventsAsync(MinIndex);
        }

        Task<ICollection<Event>> GetEventsAsync(long fromIndex);

        Task<(ICollection<Event>, ConcurrencyToken)> GetEventsAsync(string aggregateId);

        Task AddEventsAsync(ICollection<Event> newEvents, ConcurrencyToken token);

        Task SubscribeAsync(IEventConsumer consumer)
        {
            return SubscribeAsync(consumer, MinIndex);
        }

        Task SubscribeAsync(IEventConsumer consumer, long fromIndex);
    }
}
