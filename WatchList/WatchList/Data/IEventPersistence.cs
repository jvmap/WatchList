using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data
{
    public delegate Task AsyncEventProcessor(IReadOnlyCollection<EventEnvelope> events);

    public interface IEventPersistence
    {
        long MinIndex { get; }
        int DefaultBatchSize { get; }

        Task GetEventsAsync(AsyncEventProcessor processBatchAsync)
        {
            return GetEventsAsync(MinIndex, processBatchAsync, DefaultBatchSize);
        }

        Task GetEventsAsync(AsyncEventProcessor processBatchAsync, int batchSize)
        {
            return GetEventsAsync(MinIndex, processBatchAsync, batchSize);
        }

        Task GetEventsAsync(long fromIndex, AsyncEventProcessor processBatchAsync)
        {
            return GetEventsAsync(fromIndex, processBatchAsync, DefaultBatchSize);
        }

        Task GetEventsAsync(long fromIndex, AsyncEventProcessor processBatchAsync, int batchSize);

        Task GetEventsAsync(string aggregateId, AsyncEventProcessor processBatchAsync)
        {
            return GetEventsAsync(aggregateId, processBatchAsync, DefaultBatchSize);
        }

        Task GetEventsAsync(string aggregateId, AsyncEventProcessor processBatchAsync, int batchSize)
        {
            return GetEventsAsync(MinIndex, aggregateId, processBatchAsync, batchSize);
        }

        Task GetEventsAsync(long fromIndex, string aggregateId, AsyncEventProcessor processBatchAsync)
        {
            return GetEventsAsync(fromIndex, aggregateId, processBatchAsync, DefaultBatchSize);
        }

        Task GetEventsAsync(long fromIndex, string aggregateId, AsyncEventProcessor processBatchAsync, int batchSize);

        Task<IReadOnlyCollection<EventEnvelope>> AddEventsAsync(IReadOnlyCollection<EventEnvelope> newEvents);
    }
}
