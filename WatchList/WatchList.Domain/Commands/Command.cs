using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Domain.Entities;

namespace WatchList.Domain.Commands
{
    public abstract class Command<TEntity>
        where TEntity: Entity
    {
        public string AggregateId { get; set; }
    }
}
