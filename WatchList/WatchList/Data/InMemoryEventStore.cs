using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data
{
    /// <summary>
    /// This class is threadsafe.
    /// </summary>
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<Event> _events = new List<Event>();
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        
        public async Task AddEventAsync(Event evt)
        {
            await _lock.WaitAsync();
            try
            {
                _events.Add(evt);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
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

        public async Task<IEnumerable<Event>> GetEventsAsync(string aggregateId)
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
    }
}
