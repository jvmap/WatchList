using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Domain.Commands;
using WatchList.Domain.Entities;
using WatchList.Domain.Events;
using WatchList.DynamicDispatch;

namespace WatchList.Commands
{
    public class CommandDispatcher<TEntity>
        where TEntity: Entity
    {
        private readonly TEntity _target;
        private readonly DynamicDispatcher _dispatcher;

        public CommandDispatcher(TEntity target, DynamicDispatcher dispatcher)
        {
            this._target = target;
            this._dispatcher = dispatcher;
        }

        public IEnumerable<Event> Handle(Command<TEntity> cmd)
        {
            return _dispatcher.Dispatch<IEnumerable<Event>>(_target, nameof(Handle), cmd);
        }
    }
}
