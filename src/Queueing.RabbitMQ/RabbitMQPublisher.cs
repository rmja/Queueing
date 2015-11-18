#if DNX451
using RabbitMQ.Client;
#endif
using System;
using System.Threading.Tasks;

namespace Queueing.RabbitMQ
{
#if DNX451
	public class RabbitMQPublisher : IPublisher, IDisposable
	{
		private readonly IMessageConverter _converter;
		private readonly IModel _model;

        public RabbitMQPublisher(IRabbitMQConnectionAccessor connectionAccessor, IMessageConverter converter)
		{
			_converter = converter;
			_model = connectionAccessor.Connection.CreateModel();
		}

		public void Publish(IExchange exchange, IMessage message, string route)
		{
			var properties = _model.CreateBasicProperties();

			properties.Persistent = true;

			var body = _converter.Serialize(message);
			_model.BasicPublish(exchange.Name, route, properties, body);
		}

		public void Dispose()
		{
			_model.Dispose();
		}
	}
#else
    public class RabbitMQPublisher : IPublisher, IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Publish(IExchange exchange, IMessage message, string route)
        {
            throw new NotImplementedException();
        }
    }
#endif
}