using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data
{
    public interface IEventStore
    {
        Task AddEventAsync(Event evt);

        Task<IEnumerable<Event>> GetEventsAsync();

        Task<IEnumerable<Event>> GetEventsAsync(string aggregateId);
    }
}
