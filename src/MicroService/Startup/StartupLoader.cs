using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MicroService.Startup
{
    public class StartupLoader
    {
        private readonly IServiceProvider _services;

        public StartupLoader(IServiceProvider services)
        {
            _services = services;
        }

        public Type FindStartupType(string assemblyName)
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));

            var typeInfo = assembly.DefinedTypes.Single(x => x.Name == "Startup");

            return typeInfo.AsType();
        }

        public StartupMethods LoadMethods(Type startupType)
        {
            var configureMethodInfo = startupType.GetMethod("Configure");
            var configureServicesMethodInfo = startupType.GetMethod("ConfigureServices");

            if (configureMethodInfo == null)
            {
                throw new ArgumentException("Configure method is not present in startup.");
            }

            if (configureServicesMethodInfo == null)
            {
                throw new ArgumentException("ConfigureServices method is not present in startup.");
            }

            var configureMethod = new ConfigureBuilder(configureMethodInfo);
            var configureServicesMethod = new ConfigureServicesBuilder(configureServicesMethodInfo);

            object startupInstance = ActivatorUtilities.GetServiceOrCreateInstance(_services, startupType);

            return new StartupMethods(configureMethod.Build(startupInstance), configureServicesMethod.Build(startupInstance));
        }
    }

    public class StartupMethods
    {
        public Action<IMicroServiceBuilder> ConfigureDelegate { get; }
        public Func<IServiceCollection, IServiceProvider> ConfigureServicesDelegate { get; }

        internal static Func<IServiceCollection, IServiceProvider> DefaultBuildServiceProvider = s => s.BuildServiceProvider();

        public StartupMethods(Action<IMicroServiceBuilder> configure, Func<IServiceCollection, IServiceProvider> configureServices)
        {
            ConfigureDelegate = configure;
            ConfigureServicesDelegate = configureServices ?? DefaultBuildServiceProvider;
        }
    }
}
