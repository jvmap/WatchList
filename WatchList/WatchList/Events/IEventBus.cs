using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public interface IEventBus
    {
        Task PublishEventAsync(Event evt);

        Task SubscribeAsync(IEventConsumer consumer);
    }
}
