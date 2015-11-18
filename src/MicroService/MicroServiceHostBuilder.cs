using System;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Logging;
using System.Reflection;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime;
using MicroService.Startup;
using MicroService.Internal;

namespace MicroService
{
    /// <summary>
    /// https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting/WebHostBuilder.cs
    /// </summary>
    public class MicroServiceHostBuilder
    {
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

        // Only one of these should be set.
        private Type _startupType { get; set; }
        private string _startupAssemblyName { get; set; }

        public MicroServiceHostBuilder(IServiceProvider services, IConfiguration config)
        {
            _services = services;
            _config = config;
        }

        public IMicroServiceEngine Build()
        {
            var hostingServices = BuildHostingServices();

            var hostingContainer = hostingServices.BuildServiceProvider();

            var appEnvironment = hostingContainer.GetRequiredService<IApplicationEnvironment>();
            var startupLoader = hostingContainer.GetRequiredService<StartupLoader>();

            var engine = new HostingEngine(hostingServices, startupLoader, _config);

            // Only one of these should be set, but they are used in priority
            engine.StartupType = _startupType;
            engine.StartupAssemblyName = _startupAssemblyName ?? appEnvironment.ApplicationName;

            return engine;
        }

        private IServiceCollection BuildHostingServices()
        {
            var services = new ServiceCollection();

            // Import from manifest
            var manifest = _services.GetService<IRuntimeServices>();
            if (manifest != null)
            {
                foreach (var service in manifest.Services)
                {
                    services.AddTransient(service, sp => _services.GetService(service));
                }
            }

            services.AddInstance(_loggerFactory);
            services.AddTransient<StartupLoader>();
            services.AddSingleton<IMessageContextAccessor, MessageContextAccessor>();
            services.AddLogging();

            return services;
        }
    }
}