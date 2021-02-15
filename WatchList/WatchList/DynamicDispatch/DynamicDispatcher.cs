using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WatchList.Services;

namespace WatchList.DynamicDispatch
{
    public class DynamicDispatcher
    {
        Memoizator<(Type targetType, string methodName, Type parameterType, Type resultType), MethodInfo> _methodInfoMemo;
        
        private enum DispatchType
        {
            Required,
            Optional
        }

        public DynamicDispatcher()
        {
            _methodInfoMemo = new Memoizator<(Type targetType, string methodName, Type parameterType, Type resultType), MethodInfo>(
                @in => GetMethodInfo(@in.targetType, @in.methodName, @in.parameterType, @in.resultType));

            static MethodInfo GetMethodInfo(Type targetType, string methodName, Type parameterType, Type resultType)
            {
                MethodInfo[] matches = targetType
                                .GetMethods()
                                .Where(method => Matches(method, methodName, parameterType, resultType))
                                .ToArray();
                if (matches.Length > 1)
                    throw new InvalidOperationException(
                        $"Object of type {targetType} has more than one method {resultType} {methodName}({parameterType})" +
                        $", where exactly one was expected.");
                return matches.SingleOrDefault();
            }

            static bool Matches(
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

            static bool HasSingleMatchingParameter(MethodInfo method, Type paramType)
            {
                ParameterInfo[] @params = method.GetParameters();
                return @params.Length == 1
                    && @params[0].ParameterType == paramType;
            }
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

        private object PrivateDispatch(
            object target, 
            string methodName, 
            object parameter, 
            Type resultType,
            DispatchType dispatchType,
            object defaultValue = null)
        {
            MethodInfo method = _methodInfoMemo.GetValue((target.GetType(), methodName, parameter.GetType(), resultType));
            if (method == null)
            {
                if (dispatchType == DispatchType.Required)
                    throw new InvalidOperationException(
                        $"Object of type {target.GetType()} does not have required method " +
                        $"{resultType} {methodName}({parameter.GetType()}).");
                else
                    return defaultValue;
            }
            else
                return method.Invoke(target, new object[] { parameter });
        }
    }
}
