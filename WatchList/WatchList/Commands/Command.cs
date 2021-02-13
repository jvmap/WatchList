using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchList.Commands
{
    public abstract class Command
    {
        public string AggregateId { get; set; }
    }
}
