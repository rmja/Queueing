using Queueing;
using Queueing.RabbitMQ;
using System;
using System.Linq;

namespace Microsoft.Framework.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddRabbitMQQueueing(this IServiceCollection services)
		{
            services.AddSingleton<IRabbitMQConnectionAccessor, RabbitMQConnectionAccessor>();
			services.AddScoped<IBroker, RabbitMQBroker>();
			services.AddScoped<IConsumer, RabbitMQConsumer>();
			services.AddScoped<IPublisher, RabbitMQPublisher>();
            services.AddScoped<IRemoteProcedureCallClient, RabbitMQRemoteProcedureCallClient>();

			return services;
		}
	}
}