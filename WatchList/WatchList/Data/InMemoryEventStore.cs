using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Events;

namespace WatchList.Data
{
    public class InMemoryEventStore : IEventStore
    {
        private static readonly List<EventInfo> _events = new List<EventInfo>();
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        
        public async Task AddEventAsync(IEvent evt)
        {
            var nfo = new EventInfo(evt);
            await _lock.WaitAsync();
            try
            {
                _events.Add(nfo);
            }
            finally
            {
                _lock.Release();
            }
        }

        class EventInfo
        {
            public EventInfo(IEvent evt)
            {
                AggregateId = evt.AggregateId;
                Name = evt.Name;
                EventData = evt.EventData.ToList();
            }
            
            string AggregateId { get; }
            
            string Name { get; }

            IEnumerable<(string, string)> EventData { get; }
        }
    }
}
