using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Domain.Events;
using WatchList.DynamicDispatch;

namespace WatchList.Events
{
    public class EventDispatcher : IEventConsumer
    {
        private readonly object _target;
        private readonly DynamicDispatcher _dispatcher;

        public EventDispatcher(object target, DynamicDispatcher dispatcher)
        {
            this._target = target;
            this._dispatcher = dispatcher;
        }

        public void OnNext(Event evt)
        {
            _dispatcher.Dispatch(_target, nameof(OnNext), evt);
        }

        public Task OnNextAsync(Event evt)
        {
            return _dispatcher.DispatchOptional<Task>(_target, nameof(OnNextAsync), evt, Task.CompletedTask);
        }
    }
}
