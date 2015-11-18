using MicroService.Listener;
using MicroService.Middlewares;
using Microsoft.Framework.DependencyInjection;
using Queueing;

namespace MicroService
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