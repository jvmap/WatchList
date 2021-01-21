using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public interface IEventBus
    {
        void PublishEvent(IEvent evt);
    }
}
