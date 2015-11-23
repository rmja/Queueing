using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MicroService.Startup;
using MicroService.Hosting;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace MicroService
{
    /// <summary>
    /// https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting/Program.cs
    /// </summary>
    public class Program
    {
        private readonly IServiceProvider _serviceProvider;

        public Program(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Main()
        {
            var builder = new ConfigurationBuilder();
            var config = builder.Build();

            var host = new MicroServiceHostBuilder(_serviceProvider, config).Build();

            using (host.Start())
            {
                Console.WriteLine("Started");
                var appShutdownService = host.ApplicationServices.GetRequiredService<IApplicationShutdown>();
                Console.CancelKeyPress += delegate { appShutdownService.RequestShutdown(); };
                appShutdownService.ShutdownRequested.WaitHandle.WaitOne();
            }
        }
    }
}
