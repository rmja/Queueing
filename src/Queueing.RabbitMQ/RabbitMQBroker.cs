#if DNX451
using RabbitMQ.Client;
#endif
using System;
using System.Collections.Generic;

namespace Queueing.RabbitMQ
{
#if DNX451
	public class RabbitMQBroker : IBroker
    {
		private readonly IConnection _connection;
		private readonly IModel _model;
        
        public RabbitMQBroker()
        {
			var factory = new ConnectionFactory()
			{
				HostName = "localhost"
			};

			_connection = factory.CreateConnection();
			_model = _connection.CreateModel();
		}

		public void DeclareExchange(IExchange exchange)
		{
			var args = new Dictionary<string, object>();

			if (exchange.AlternateExchange != null)
			{
				// http://www.rabbitmq.com/ae.html

				args.Add("alternate-exchange", exchange.AlternateExchange.Name);
            }

			_model.ExchangeDeclare(exchange.Name, "topic", true, false, args);
		}

		public void DeclareQueue(IQueue queue)
		{
			var args = new Dictionary<string, object>();

			if (queue.DeadLetterExchange != null)
			{
				// https://www.rabbitmq.com/dlx.html

				args.Add("x-dead-letter-exchange", queue.DeadLetterExchange.Name);
            }

            _model.QueueDeclare(queue.Name, true, false, false, args);
		}

		public void Bind(IQueue destination, IExchange source, params string[] topics)
		{
			foreach (var topic in topics)
			{
				_model.QueueBind(destination.Name, source.Name, topic);
			}
		}

		public void Bind(IExchange destination, IExchange source, params string[] topics)
		{
			foreach (var topic in topics)
			{
                _model.ExchangeBind(destination.Name, source.Name, topic);
			}
		}
	}
#else
    public class RabbitMQBroker : IBroker
    {
        public void Bind(IExchange destination, IExchange source, params string[] topics)
        {
            throw new NotImplementedException();
        }

        public void Bind(IQueue destination, IExchange source, params string[] topics)
        {
            throw new NotImplementedException();
        }

        public void DeclareExchange(IExchange exchange)
        {
            throw new NotImplementedException();
        }

        public void DeclareQueue(IQueue queue)
        {
            throw new NotImplementedException();
        }
    }
#endif
}