using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Domain.Commands;
using WatchList.Domain.Entities;
using WatchList.Domain.Events;

namespace WatchList.Commands
{
    public class CommandDispatcher<TEntity>
        where TEntity: Entity
    {
        private readonly TEntity _target;

        public CommandDispatcher(TEntity target)
        {
            this._target = target;
        }

        public IEnumerable<Event> Handle(Command<TEntity> cmd)
        {
            MethodInfo[] methods = GetApplicableMethods(cmd, nameof(Handle), typeof(IEnumerable<Event>))
                .ToArray();
            if (methods.Length == 0)
                throw new InvalidOperationException(
                    $"Object of type {_target.GetType()} does not have required method IEnumerable<Event> Handle({cmd.GetType()}).");
            else if (methods.Length == 1)
                return InvokeMethod(cmd, methods[0]);
            else
                throw new InvalidOperationException(
                    $"Object of type {_target.GetType()} has more than one method IEnumerable<Event> Handle({cmd.GetType()})" +
                    $", where exactly one was expected.");
        }

        private IEnumerable<MethodInfo> GetApplicableMethods(
            Command<TEntity> cmd,
            string expectedMethodName,
            Type expectedReturnType)
        {
            Type cmdType = cmd.GetType();
            Type targetType = _target.GetType();
            IEnumerable<MethodInfo> methods = targetType
                .GetMethods()
                .Where(method => IsApplicable(method, cmdType, expectedMethodName, expectedReturnType))
                .ToList();
            return methods;
        }

        private static bool IsApplicable(
            MethodInfo method, 
            Type cmdType, 
            string expectedMethodName, 
            Type expectedReturnType)
        {
            if (method.Name == expectedMethodName
                && method.ReturnType == expectedReturnType)
            {
                return HasSingleMatchingParameter(method, cmdType);
            }
            return false;
        }

        private static bool HasSingleMatchingParameter(MethodInfo method, Type cmdType)
        {
            ParameterInfo[] @params = method.GetParameters();
            return @params.Length == 1
                && @params[0].ParameterType == cmdType;
        }

        private IEnumerable<Event> InvokeMethod(Command<TEntity> cmd, MethodInfo method)
        {
            return (IEnumerable<Event>) method.Invoke(_target, new object[] { cmd });
        }
    }
}
