using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Events;

namespace WatchList.Data
{
    /// <summary>
    /// This class is threadsafe.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<EventInfo> _events = new List<EventInfo>();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        
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

        public async Task<IEnumerable<IEvent>> GetEventsAsync()
        {
            await _lock.WaitAsync();
            try
            {
                return _events.ToList();
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(string aggregateId)
        {
            await _lock.WaitAsync();
            try
            {
                return _events
                    .Where(ev => ev.AggregateId == aggregateId)
                    .ToList();
            }
            finally
            {
                _lock.Release();
            }
        }

        class EventInfo : IEvent
        {
            public EventInfo(IEvent evt)
            {
                AggregateId = evt.AggregateId;
                Name = evt.Name;
                EventData = evt.EventData.ToList();
            }
            
            public string AggregateId { get; }
            
            public string Name { get; }

            public IEnumerable<(string, string)> EventData { get; }
        }
    }
}
