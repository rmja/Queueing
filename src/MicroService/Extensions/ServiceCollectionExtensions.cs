using MicroService.Listener;
using MicroService.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Queueing;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMicroServiceConsumer<TQueue>(this IServiceCollection self)
            where TQueue : IQueue, new()
        {
            self.AddSingleton<IListenerEngine, ComsumerListenerEngine<TQueue>>();
            self.AddSingleton<MicroServiceConsumerMiddleware>();
        }
    }
}