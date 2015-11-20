#if DNX451
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Queueing.RabbitMQ
{
#if DNX451
    public class RabbitMQConsumer : IConsumer, IDisposable
	{
		private readonly IModel _model;
        private QueueingBasicConsumer _rabbitConsumer;
        private IQueue _queue;
        
        public RabbitMQConsumer(IRabbitMQConnectionAccessor connectionAccessor)
		{
			_model = connectionAccessor.Connection.CreateModel();
		}

        public IConsumeInfo Consume(IQueue queue, TimeSpan timeout)
        {
            if (_queue == null)
            {
                _queue = queue;

                _rabbitConsumer = new QueueingBasicConsumer(_model);
                _model.BasicConsume(queue.Name, false, _rabbitConsumer);
                _model.BasicQos(0, 1, false);
            }
            else if (_queue != queue)
            {
                throw new NotSupportedException();
            }

            BasicDeliverEventArgs eventArgs;
            if (_rabbitConsumer.Queue.Dequeue((int)timeout.TotalMilliseconds, out eventArgs))
            {
                return new RabbitMQConsumeInfo(eventArgs);
            }
            return null;
        }

        public void SendRepy(IConsumeInfo consumed, byte[] body)
        {
            var eventArgs = ((RabbitMQConsumeInfo)consumed).EventArgs;

            var properties = _model.CreateBasicProperties();
            properties.CorrelationId = eventArgs.BasicProperties.CorrelationId;
            _model.BasicPublish("", eventArgs.BasicProperties.ReplyTo, properties, body);
        }

        public void Ack(IConsumeInfo consumed)
        {
            var eventArgs = ((RabbitMQConsumeInfo)consumed).EventArgs;

            _model.BasicAck(eventArgs.DeliveryTag, false);
        }

        public void Reject(IConsumeInfo consumed, bool requeue)
        {
            var eventArgs = ((RabbitMQConsumeInfo)consumed).EventArgs;

            _model.BasicReject(eventArgs.DeliveryTag, requeue);
        }

        public void Dispose()
		{
			_model.Dispose();
		}
    }
#else
    public class RabbitMQConsumer : IConsumer, IDisposable
    {
        public void Ack(IConsumeInfo consumed)
        {
            throw new NotImplementedException();
        }

        public IConsumeInfo Consume(IQueue queue, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Reject(IConsumeInfo consumed, bool requeue)
        {
            throw new NotImplementedException();
        }

        public void SendRepy(IConsumeInfo consumed, byte[] body)
        {
            throw new NotImplementedException();
        }
    }
#endif

}