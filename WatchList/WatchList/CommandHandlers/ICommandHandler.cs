using System.Collections.Generic;
using WatchList.Domain.Events;

namespace WatchList.CommandHandlers
{
    public interface ICommandHandler<TCommand, TEntity>
    {
        IEnumerable<Event> Handle(TEntity entity, TCommand cmd);
    }
}