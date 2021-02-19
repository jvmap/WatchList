using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Data;
using WatchList.Domain.Events;

namespace WatchList.Events
{
    public class InMemoryEventStore : IEventStore
    {
        private static readonly HashSet<IEventConsumer> _consumers = new HashSet<IEventConsumer>();
        private static readonly SemaphoreSlim _consumersLock = new SemaphoreSlim(1, 1);
        private readonly IEventPersistence _persistence;
        public long MinIndex => _persistence.MinIndex;

        public InMemoryEventStore(IEventPersistence persistence)
        {
            this._persistence = persistence;
        }

        public async Task AddEventsAsync(ICollection<Event> events, ConcurrencyToken token)
        {
            IReadOnlyCollection<EventEnvelope> envelopes = events
                .Select((evt, idx) => new EventEnvelope
                {
                    Version = token.Version + idx + 1,
                    Event = evt,
                    Timestamp = DateTimeOffset.Now
                })
                .ToList();
            envelopes = await _persistence.AddEventsAsync(envelopes);
            // process events asynchronously
            // FIXME. Events could get out of order here,
            // also not threadsafe.
            // It looks like an intermediary queue per consumer is needed.
            await PublishEventsAsync(envelopes);
        }

        public async Task<ICollection<Event>> GetEventsAsync(long fromIndex)
        {
            var result = new List<Event>();
            await _persistence.GetEventsAsync(fromIndex, envelopes => {
                result.AddRange(envelopes.Select(envelope => envelope.Event));
                return Task.CompletedTask;
            });
            return result;
        }

        public async Task<(ICollection<Event>, ConcurrencyToken)> GetEventsAsync(string aggregateId)
        {
            var events = new List<Event>();
            int version = 0;
            await _persistence.GetEventsAsync(aggregateId, envelopes => {
                version = envelopes.Last().Version;
                events.AddRange(envelopes.Select(envelope => envelope.Event));
                return Task.CompletedTask;
            });
            var token = new ConcurrencyToken { Version = version };
            return (events, token);
        }

        private async Task PublishEventsAsync(IReadOnlyCollection<EventEnvelope> envelopes)
        {
            var batch = new List<(long index, Event)>(envelopes.Count);
            batch.AddRange(envelopes
                .Select(envelope => (envelope.Index, envelope.Event)));
            IReadOnlyCollection<IEventConsumer> consumers;
            await _consumersLock.WaitAsync();
            try
            {
                consumers = _consumers.ToList();
            }
            finally
            {
                _consumersLock.Release();
            }
            foreach (IEventConsumer consumer in consumers)
            {
                await consumer.ProcessBatchAsync(batch);
            }
        }

        public async Task SubscribeAsync(IEventConsumer consumer, long fromIndex)
        {
            long proxyStartIndex = fromIndex;
            await _persistence.GetEventsAsync(fromIndex, async envelopes => {
                await consumer.ProcessBatchAsync(envelopes
                    .Select(envelope => (envelope.Index, envelope.Event))
                    .ToList());
                proxyStartIndex = envelopes.Last().Index;
            });
            var proxy = new ConsumerProxy(consumer, proxyStartIndex, _persistence);
            await _consumersLock.WaitAsync();
            try
            {
                _consumers.Add(proxy);
            }
            finally
            {
                _consumersLock.Release();
            }
        }

        class ConsumerProxy : IEventConsumer
        {
            private readonly IEventConsumer _inner;
            private long _lastProcessedIndex;
            private readonly IEventPersistence _persistence;

            public ConsumerProxy(IEventConsumer inner, long lastProcessedIndex, IEventPersistence persistence)
            {
                this._inner = inner;
                this._lastProcessedIndex = lastProcessedIndex;
                this._persistence = persistence;
            }

            public async Task ProcessBatchAsync(IReadOnlyCollection<(long index, Event)> batch)
            {
                long firstIndex = batch.First().index;
                if (firstIndex - _lastProcessedIndex > 1)
                {
                    await _persistence.GetEventsAsync(_lastProcessedIndex + 1, async envelopes => {
                        await _inner.ProcessBatchAsync(envelopes
                            .Select(env => (env.Index, env.Event))
                            .ToList());
                        _lastProcessedIndex = envelopes.Last().Index;
                    });
                }
                if (firstIndex - _lastProcessedIndex > 1)
                    return; // still missing events, better luck next time.
                await _inner.ProcessBatchAsync(batch
                    .Skip((int)(_lastProcessedIndex - firstIndex))
                    .ToList());
                _lastProcessedIndex = batch.Last().index;
            }
        }
    }
}
