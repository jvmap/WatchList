using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public interface IEvent
    {
        string Name { get; }

        string AggregateId { get; }

        IEnumerable<(string, string)> EventData { get; }
    }
}
