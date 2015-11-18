using MicroService.Middlewares;
using Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    public static class MicroServiceBuilderExtensions
    {
        public static void UseMicroServiceConsumer(this IMicroServiceBuilder self, Action<MicroServiceConsumerOptions> configurator)
        {
            var options = new MicroServiceConsumerOptions();
            configurator(options);
            self.UseMiddleware<MicroServiceConsumerMiddleware>(options);
        }

        public static void UseMicroServiceConsumer(this IMicroServiceBuilder self, MicroServiceConsumerOptions options)
        {
            self.UseMiddleware<MicroServiceConsumerMiddleware>(options);
        }
    }
}
