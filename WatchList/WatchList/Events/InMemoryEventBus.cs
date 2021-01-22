using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public class InMemoryEventBus : IEventBus
    {
        private static readonly HashSet<IEventConsumer> _consumers = new HashSet<IEventConsumer>();
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public async Task PublishEventAsync(IEvent evt)
        {
            await _lock.WaitAsync();
            try
            {
                foreach (IEventConsumer consumer in _consumers)
                {
                    await consumer.OnNextAsync(evt);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SubscribeAsync(IEventConsumer consumer)
        {
            await _lock.WaitAsync();
            try
            {
                _consumers.Add(consumer);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
