#if DNX451
using Queueing.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queueing
{
#if DNX451
    public class RabbitMQRemoteProcedureCallClient : IRemoteProcedureCallClient, IDisposable
    {
        private readonly IMessageConverter _converter;
        private readonly IModel _model;
        private readonly string _replyQueueName;
        private readonly QueueingBasicConsumer _consumer;

        public RabbitMQRemoteProcedureCallClient(IRabbitMQConnectionAccessor connectionAccessor, IMessageConverter converter)
        {
            _converter = converter;
            _model = connectionAccessor.Connection.CreateModel();
            _replyQueueName = _model.QueueDeclare();
            _consumer = new QueueingBasicConsumer(_model);

            _model.BasicConsume(_replyQueueName, true, _consumer);
        }

        public Task<TReply> CallAsync<TReply>(IExchange exchange, IMessage request, string route)
            where TReply : IMessage
        {
            var correlationId = Guid.NewGuid().ToString();
            var properties = _model.CreateBasicProperties();

            properties.Persistent = true;
            properties.ReplyTo = _replyQueueName;
            properties.CorrelationId = correlationId;

            var requestBody = _converter.Serialize(request);
            _model.BasicPublish(exchange.Name, route, properties, requestBody);

            var task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var eventArgs = _consumer.Queue.Dequeue();

                    if (eventArgs.BasicProperties.CorrelationId == correlationId)
                    {
                        var replyBody = eventArgs.Body;
                        var message = _converter.Deserialize<TReply>(replyBody);

                        return message;
                    }
                }
            });

            return task;
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
#else
    public class RabbitMQRemoteProcedureCallClient : IRemoteProcedureCallClient, IDisposable
    {
        public Task<TReply> CallAsync<TReply>(IExchange exchange, IMessage request, string route) where TReply : IMessage
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
#endif
}
