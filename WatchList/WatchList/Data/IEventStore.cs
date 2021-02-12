using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Events;

namespace WatchList.Data
{
    public interface IEventStore
    {
        Task AddEventAsync(IEvent evt);

        Task<IEnumerable<IEvent>> GetEventsAsync();

        Task<IEnumerable<IEvent>> GetEventsAsync(string aggregateId);
    }
}
