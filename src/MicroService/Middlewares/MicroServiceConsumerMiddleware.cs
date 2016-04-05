using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroService.Middlewares
{
    public class MicroServiceConsumerMiddleware
    {
        private readonly MicroServiceRequestDelegate _next;
        private readonly MicroServiceConsumerOptions _options;

        private readonly ILogger _logger;
        private readonly MethodInfo _handleMethodInfo;
        private readonly MethodInfo _extractGenericAsyncResultMethodInfo = typeof(MicroServiceConsumerMiddleware).GetMethod(nameof(ExtractGenericAsyncResult), BindingFlags.Static | BindingFlags.NonPublic);

        public MicroServiceConsumerMiddleware(MicroServiceRequestDelegate next, MicroServiceConsumerOptions options, ILoggerFactory loggerFactory)
        {
            _next = next;
            _options = options;
            _logger = loggerFactory.CreateLogger<MicroServiceConsumerMiddleware>();

            if (_options.HandlerType == null)
            {
                throw new Exception("No handler type specified.");
            }

            _handleMethodInfo = _options.HandlerType.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public);

            if (_handleMethodInfo == null)
            {
                throw new Exception("Unable to find Handle.");
            }
        }

       

        public async Task Invoke(MessageContext context, IServiceProvider services, IMessageConverter messageConverter)
        {
            if (_options.Routes == null || _options.Routes.Any(pattern => RouteMatching.RouteMatchesPattern(context.Route, pattern)))
            {
                var handler = services.GetRequiredService(_options.HandlerType);

                var parameters = _handleMethodInfo.GetParameters();

                if (!parameters[0].ParameterType.GetTypeInfo().IsAssignableFrom(_options.MessageType))
                {
                    throw new Exception("Handlers Handle method must take first argument of assigned message type.");
                }

                var arguments = new object[parameters.Length];
                arguments[0] = messageConverter.Deserialize(context.Body, _options.MessageType);
                for (var index = 1; index < parameters.Length; index++)
                {
                    var serviceType = parameters[index].ParameterType;

                    if (serviceType == typeof(string) && parameters[index].Name == "route")
                    {
                        arguments[index] = context.Route;
                    }
                    else
                    {
                        arguments[index] = services.GetService(serviceType);
                        if (arguments[index] == null)
                        {
                            throw new Exception(string.Format("No service for type '{0}' has been registered.", serviceType));
                        }
                    }
                }

                object result = null;
                try
                {
                    if (_handleMethodInfo.ReturnType == typeof(Task))
                    {
                        var task = (Task)_handleMethodInfo.Invoke(handler, arguments);
                        await task;
                    }
                    else if (_handleMethodInfo.ReturnType.IsGenericType && _handleMethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var task = (Task)_handleMethodInfo.Invoke(handler, arguments);
                        await task;
                        result = _extractGenericAsyncResultMethodInfo.MakeGenericMethod(task.GetType().GetGenericArguments()).Invoke(this, new object[] { task });
                    }
                    else
                    {
                        result = _handleMethodInfo.Invoke(handler, arguments);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Handler cannot handle message, rejecting without re-queueing.", e);

                    context.Ack = AckType.Reject;
                }

                if (result != null)
                {
                    if (result is bool)
                    {
                        context.Ack = (bool)result ? AckType.Ack : AckType.RejectButRequeue;
                    }
                    else if (result is AckType)
                    {
                        context.Ack = (AckType)result;
                    }
                    else if (result is IMessage)
                    {
                        context.Ack = AckType.Ack;
                        context.ReplyBody = messageConverter.Serialize((IMessage)result);
                    }
                    else
                    {
                        throw new Exception("Unsupported handler return type.");
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }

        private static object ExtractGenericAsyncResult<T>(Task task)
        {
            var result = ((Task<T>)task).Result;
            return result;
        }
    }
}
