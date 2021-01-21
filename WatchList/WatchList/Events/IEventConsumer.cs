using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public interface IEventConsumer
    {
        void OnNext(IEvent evt);
    }
}
