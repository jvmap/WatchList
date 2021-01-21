using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public abstract class Event: IEvent
    {
        public string Name => this.GetType().Name;

        public abstract string AggregateId { get; }
        public abstract IEnumerable<(string, string)> EventData { get; }
    }
}
