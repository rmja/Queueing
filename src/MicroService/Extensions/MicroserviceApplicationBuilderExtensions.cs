using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroService
{
    /// <summary>
    /// https://github.com/aspnet/HttpAbstractions/blob/dev/src/Microsoft.AspNet.Http.Abstractions/Extensions/UseExtensions.cs
    /// https://github.com/aspnet/HttpAbstractions/blob/dev/src/Microsoft.AspNet.Http.Abstractions/Extensions/UseMiddlewareExtensions.cs
    /// </summary>
    public static class MicroserviceApplicationBuilderUseExtensions
    {
        public static IMicroServiceBuilder Use(this IMicroServiceBuilder builder, Func<MessageContext, Func<Task>, Task> middleware)
        {
            return builder.Use(next =>
            {
                return context =>
                {
                    Func<Task> simpleNext = () => next(context);
                    return middleware(context, simpleNext);
                };
            });
        }

        public static IMicroServiceBuilder UseMiddleware<T>(this IMicroServiceBuilder builder, params object[] args)
        {
            return builder.UseMiddleware(typeof(T), args);
        }

        public static IMicroServiceBuilder UseMiddleware(this IMicroServiceBuilder builder, Type middleware, params object[] args)
        {
            var applicationServices = builder.ApplicationServices;
            return builder.Use(next =>
            {
                var instance = ActivatorUtilities.CreateInstance(builder.ApplicationServices, middleware, new[] { next }.Concat(args).ToArray());
                var methodinfo = middleware.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
                var parameters = methodinfo.GetParameters();
                if (parameters[0].ParameterType != typeof(MessageContext))
                {
                    throw new Exception("Middleware Invoke method must take first argument of MicroServiceContext");
                }
                if (parameters.Length == 1)
                {
                    return (MicroServiceRequestDelegate)methodinfo.CreateDelegate(typeof(MicroServiceRequestDelegate), instance);
                }
                return context =>
                {
                    var serviceProvider = context.RequestServices ?? context.ApplicationServices ?? applicationServices;
                    if (serviceProvider == null)
                    {
                        throw new Exception("IServiceProvider is not available");
                    }
                    var arguments = new object[parameters.Length];
                    arguments[0] = context;
                    for (var index = 1; index != parameters.Length; ++index)
                    {
                        var serviceType = parameters[index].ParameterType;
                        arguments[index] = serviceProvider.GetService(serviceType);
                        if (arguments[index] == null)
                        {
                            throw new Exception(string.Format("No service for type '{0}' has been registered.", serviceType));
                        }
                    }
                    return (Task)methodinfo.Invoke(instance, arguments);
                };
            });
        }
    }
}
