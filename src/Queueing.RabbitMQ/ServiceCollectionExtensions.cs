using Queueing;
using Queueing.RabbitMQ;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRabbitMQQueueing(this IServiceCollection services, Action<RabbitMQOptions> setupAction = null)
		{
            services.AddOptions();

            services.AddSingleton<IRabbitMQConnectionAccessor, RabbitMQConnectionAccessor>();
			services.AddScoped<IBroker, RabbitMQBroker>();
			services.AddScoped<IConsumer, RabbitMQConsumer>();
			services.AddScoped<IPublisher, RabbitMQPublisher>();
            services.AddScoped<IRemoteProcedureCallClient, RabbitMQRemoteProcedureCallClient>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

			return services;
		}
	}
}