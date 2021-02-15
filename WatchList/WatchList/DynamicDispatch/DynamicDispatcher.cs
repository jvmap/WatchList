using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WatchList.DynamicDispatch
{
    public class DynamicDispatcher
    {
        private enum DispatchType
        {
            Required,
            Optional
        }

        public void Dispatch(object target, string methodName, object parameter)
        {
            PrivateDispatch(target, methodName, parameter, typeof(void), DispatchType.Required);
        }
        public void DispatchOptional(object target, string methodName, object parameter)
        {
            PrivateDispatch(target, methodName, parameter, typeof(void), DispatchType.Optional);
        }

        public TResult Dispatch<TResult>(object target, string methodName, object parameter)
        {
            return (TResult) PrivateDispatch(target, methodName, parameter, typeof(TResult), DispatchType.Required);
        }

        public TResult DispatchOptional<TResult>(object target, string methodName, object parameter, TResult defaultValue)
        {
            return (TResult)PrivateDispatch(target, methodName, parameter, typeof(TResult), DispatchType.Optional, defaultValue);
        }

        private static object PrivateDispatch(
            object target, 
            string methodName, 
            object parameter, 
            Type resultType,
            DispatchType dispatchType,
            object defaultValue = null)
        {
            MethodInfo[] methods = GetApplicableMethods(target, methodName, parameter, resultType);
            if (methods.Length == 0)
            {
                if (dispatchType == DispatchType.Required)
                    throw new InvalidOperationException(
                        $"Object of type {target.GetType()} does not have required method " +
                        $"{resultType} {methodName}({parameter.GetType()}).");
                else
                    return defaultValue;
            }
            else if (methods.Length == 1)
                return InvokeMethod(target, methods[0], parameter);
            else
                throw new InvalidOperationException(
                    $"Object of type {target.GetType()} has more than one method {resultType} {methodName}({parameter.GetType()})" +
                    $", where exactly one was expected.");
        }

        private static MethodInfo[] GetApplicableMethods(
            object target,
            string methodName,
            object parameter,
            Type resultType)
        {
            Type paramType = parameter.GetType();
            Type targetType = target.GetType();
            return targetType
                .GetMethods()
                .Where(method => Matches(method, methodName, paramType, resultType))
                .ToArray();
        }

        private static bool Matches(
            MethodInfo method,
            string methodName,
            Type paramType,
            Type expectedReturnType)
        {
            if (method.Name == methodName
                && method.ReturnType == expectedReturnType)
            {
                return HasSingleMatchingParameter(method, paramType);
            }
            return false;
        }

        private static bool HasSingleMatchingParameter(MethodInfo method, Type paramType)
        {
            ParameterInfo[] @params = method.GetParameters();
            return @params.Length == 1
                && @params[0].ParameterType == paramType;
        }

        private static object InvokeMethod(object target, MethodInfo method, object argument)
        {
            return method.Invoke(target, new object[] { argument });
        }
    }
}
