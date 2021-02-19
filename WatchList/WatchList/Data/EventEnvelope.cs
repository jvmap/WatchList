using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Data
{
    public class EventEnvelope
    {
        public long Index { get; init; } // sequence number within the partition. database-generated.

        public string AggregateId => Event.AggregateId;

        public int Version { get; init; } // version of the aggregate.
        
        public Event Event { get; init; }

        public DateTimeOffset Timestamp { get; init; }
    }
}
