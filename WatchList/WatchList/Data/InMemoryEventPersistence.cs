using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Common;
using WatchList.Domain.Events;

namespace WatchList.Data
{
    /// <summary>
    /// This class is threadsafe.
    /// </summary>
    public class InMemoryEventPersistence : IEventPersistence
    {
        private readonly List<EventEnvelope> _events = new List<EventEnvelope>();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public long MinIndex => 0;
        public int DefaultBatchSize => 1024;

        public async Task<IReadOnlyCollection<EventEnvelope>> AddEventsAsync(IReadOnlyCollection<EventEnvelope> newEvents)
        {
            await _lock.WaitAsync();
            try
            {
                long startIdx = _events.Count + MinIndex;
                var result = newEvents.Select((envelope, idx) => new EventEnvelope
                    {
                        Event = envelope.Event,
                        Index = startIdx + idx,
                        Version = envelope.Version,
                        Timestamp = envelope.Timestamp
                    })
                    .ToList();
                _events.AddRange(result);
                return result;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task GetEventsAsync(long fromIndex, AsyncEventProcessor process, int batchSize)
        {
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
            int listIndex = (int)fromIndex;
            EventEnvelope[] batch = null;
            int numElements;
            while (true)
            {
                await _lock.WaitAsync();
                try
                {
                    int remainingElements = _events.Count - listIndex;
                    if (remainingElements == 0)
                        break;
                    numElements = Math.Min(remainingElements, batchSize);
                    if (batch == null)
                        batch = new EventEnvelope[numElements];
                    _events.CopyTo(listIndex, batch, 0, numElements);
                    listIndex += numElements;
                }
                finally
                {
                    _lock.Release();
                }
                await process(SubArray.Take(batch, numElements));
            }
        }

        public async Task GetEventsAsync(long fromIndex, string aggregateId, AsyncEventProcessor process, int batchSize)
        {
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
            int listIndex = (int)fromIndex;
            EventEnvelope[] batch = new EventEnvelope[batchSize];
            int numElements;
            while (true)
            {
                await _lock.WaitAsync();
                try
                {
                    IEnumerator<EventEnvelope> enumerator = _events
                        .Skip(listIndex)
                        .Where(evt => evt.AggregateId == aggregateId)
                        .Take(batchSize)
                        .GetEnumerator();

                    numElements = 0;
                    while(enumerator.MoveNext())
                    {
                        batch[numElements] = enumerator.Current;
                        numElements++;
                    }
                    if (numElements == 0)
                        break;
                    listIndex += numElements;
                }
                finally
                {
                    _lock.Release();
                }
                await process(SubArray.Take(batch, numElements));
            }
        }
    }
}
