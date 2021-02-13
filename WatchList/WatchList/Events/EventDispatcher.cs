using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WatchList.Events
{
    public class EventDispatcher : IEventConsumer
    {
        private readonly object _target;

        public EventDispatcher(object target)
        {
            this._target = target;
        }
        
        public async Task OnNextAsync(Event evt)
        {
            Type eventType = evt.GetType();
            Type targetType = _target.GetType();
            IEnumerable<MethodInfo> methods = targetType
                .GetMethods()
                .Where(method => IsApplicable(method, eventType))
                .ToList();
            foreach (MethodInfo method in methods)
                await (Task) method.Invoke(_target, new object[] { evt });
        }

        private static bool IsApplicable(MethodInfo method, Type eventType)
        {
            if (method.Name == nameof(OnNextAsync) 
                && method.ReturnType == typeof(Task))
            {
                ParameterInfo[] @params = method.GetParameters();
                return @params.Length == 1
                    && @params[0].ParameterType == eventType;
            }
            return false;
        }
    }
}
