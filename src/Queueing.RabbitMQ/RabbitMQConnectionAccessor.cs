using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Queueing.RabbitMQ
{
    public class RabbitMQConnectionAccessor : IRabbitMQConnectionAccessor, IDisposable
    {
        public IConnection Connection { get; private set; }

        public RabbitMQConnectionAccessor()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            Connection = factory.CreateConnection();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
