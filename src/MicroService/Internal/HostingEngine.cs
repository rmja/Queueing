using MicroService.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using MicroService.Listener;
using Microsoft.Extensions.DependencyInjection;

namespace MicroService.Internal
{
    /// <summary>
    /// https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting/Internal/HostingEngine.cs
    /// </summary>
    public class HostingEngine : IMicroServiceEngine
    {
        private readonly IServiceCollection _applicationServiceCollection;
        private readonly StartupLoader _startupLoader;
        private readonly IConfiguration _config;

        private IServiceProvider _applicationServices;

        // Only one of these should be set.
        internal StartupMethods Startup { get; set; }
        internal Type StartupType { get; set; }
        internal string StartupAssemblyName { get; set; }

        // Only one of these should be set
        internal IListenerEngine Listener { get; set; }

        public HostingEngine(IServiceCollection appServices, StartupLoader startupLoader, IConfiguration config)
        {
            _config = config;
            _applicationServiceCollection = appServices;
            _startupLoader = startupLoader;
        }

        public IServiceProvider ApplicationServices
        {
            get
            {
                EnsureApplicationServices();
                return _applicationServices;
            }
        }

        public IDisposable Start()
        {
            EnsureApplicationServices();

            var application = BuildApplication();

            var logger = _applicationServices.GetRequiredService<ILogger<HostingEngine>>();
            var contextAccessor = _applicationServices.GetRequiredService<IMessageContextAccessor>();
            var listener = Listener.Start(async features =>
            {
                var context = new MessageContext(features);

                using (logger.BeginScope("Request"))
                {
                    contextAccessor.MessageContext = context;
                    await application(context);
                }
            });

            return listener;
        }

        private void EnsureApplicationServices()
        {
            if (_applicationServices == null)
            {
                EnsureStartup();
                _applicationServices = Startup.ConfigureServicesDelegate(_applicationServiceCollection);
            }
        }

        private void EnsureStartup()
        {
            if (Startup != null)
            {
                return;
            }

            if (StartupType == null)
            {
                StartupType = _startupLoader.FindStartupType(StartupAssemblyName);
            }

            if (StartupType == null)
            {
                throw new ArgumentException("Failed to find a startup type for the micro service.");
            }

            Startup = _startupLoader.LoadMethods(StartupType);

            if (Startup == null)
            {
                throw new ArgumentException("Failed to find a startup entry point for the micro service.");
            }
        }

        private MicroServiceRequestDelegate BuildApplication()
        {
            if (Listener == null)
            {
                Listener = _applicationServices.GetRequiredService<IListenerEngine>();
            }

            var builder = new MicroServiceBuilder(_applicationServices);

            var configure = Startup.ConfigureDelegate;
            
            configure(builder);

            return builder.Build();
        }
    }
}
