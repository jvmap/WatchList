using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Events
{
    public interface IEventConsumer
    {
        Task ProcessBatchAsync(IReadOnlyCollection<(long index, Event)> batch);
    }
}
