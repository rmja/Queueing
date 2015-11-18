using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroService.Startup
{
    public delegate void ConfigureDelegate(IMicroServiceBuilder builder);

    public class ConfigureBuilder
    {
        public MethodInfo MethodInfo { get; }

        public ConfigureBuilder(MethodInfo configure)
        {
            MethodInfo = configure;
        }

        public Action<IMicroServiceBuilder> Build(object instance) => builder => Invoke(instance, builder);

        private void Invoke(object instance, IMicroServiceBuilder builder)
        {
            var serviceProvider = builder.ApplicationServices;
            var parameterInfos = MethodInfo.GetParameters();
            var parameters = new object[parameterInfos.Length];
            for (var index = 0; index != parameterInfos.Length; ++index)
            {
                var parameterInfo = parameterInfos[index];
                if (parameterInfo.ParameterType == typeof(IMicroServiceBuilder))
                {
                    parameters[index] = builder;
                }
                else
                {
                    try
                    {
                        parameters[index] = serviceProvider.GetRequiredService(parameterInfo.ParameterType);
                    }
                    catch (Exception)
                    {
                        throw new Exception(string.Format(
                            "Could not resolve a service of type '{0}' for the parameter '{1}' of method '{2}' on type '{3}'.",
                            parameterInfo.ParameterType.FullName,
                            parameterInfo.Name,
                            MethodInfo.Name,
                            MethodInfo.DeclaringType.FullName));
                    }
                }
            }
            MethodInfo.Invoke(instance, parameters);
        }
    }
}
