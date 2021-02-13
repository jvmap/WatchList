using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Domain.Events
{
    public abstract class Event
    {
        public string AggregateId { get; init; }

        public DateTimeOffset Timestamp { get; init; }
    }
}
