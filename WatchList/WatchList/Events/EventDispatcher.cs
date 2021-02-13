using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Domain.Events;

namespace WatchList.Events
{
    public class EventDispatcher : IEventConsumer
    {
        private readonly object _target;

        public EventDispatcher(object target)
        {
            this._target = target;
        }

        public void OnNext(Event evt)
        {
            foreach (MethodInfo method in GetApplicableMethods(evt, nameof(OnNext), typeof(void)))
                InvokeMethod(evt, method);
        }

        public async Task OnNextAsync(Event evt)
        {
            foreach (MethodInfo method in GetApplicableMethods(evt, nameof(OnNextAsync), typeof(Task)))
                await InvokeMethodAsync(evt, method);
        }

        private IEnumerable<MethodInfo> GetApplicableMethods(
            Event evt,
            string expectedMethodName,
            Type expectedReturnType)
        {
            Type eventType = evt.GetType();
            Type targetType = _target.GetType();
            IEnumerable<MethodInfo> methods = targetType
                .GetMethods()
                .Where(method => IsApplicable(method, eventType, expectedMethodName, expectedReturnType))
                .ToList();
            return methods;
        }

        private static bool IsApplicable(
            MethodInfo method, 
            Type eventType, 
            string expectedMethodName, 
            Type expectedReturnType)
        {
            if (method.Name == expectedMethodName
                && method.ReturnType == expectedReturnType)
            {
                return HasSingleMatchingParameter(method, eventType);
            }
            return false;
        }

        private static bool HasSingleMatchingParameter(MethodInfo method, Type eventType)
        {
            ParameterInfo[] @params = method.GetParameters();
            return @params.Length == 1
                && @params[0].ParameterType == eventType;
        }

        private void InvokeMethod(Event evt, MethodInfo method)
        {
            method.Invoke(_target, new object[] { evt });
        }

        private Task InvokeMethodAsync(Event evt, MethodInfo method)
        {
            return (Task) method.Invoke(_target, new object[] { evt });
        }
    }
}
