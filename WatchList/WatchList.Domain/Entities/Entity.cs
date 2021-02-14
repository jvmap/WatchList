using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchList.Domain.Entities
{
    public abstract class Entity
    {
        public string AggregateId { get; init; }
    }
}
