using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public class InMemoryEventBus : IEventBus
    {
        private static readonly List<IEventConsumer> _consumers = new List<IEventConsumer>();
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public async Task PublishEventAsync(IEvent evt)
        {
            await _lock.WaitAsync();
            try
            {
                _consumers.ForEach(c => c.OnNext(evt));
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
