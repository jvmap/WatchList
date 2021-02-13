using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Domain.Events
{
    public class RatedMovieEvent : Event
    {
        public int Rating { get; init; }
    }
}
