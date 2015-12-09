using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Microsoft.Extensions.OptionsModel;

namespace Queueing.RabbitMQ
{
    public class RabbitMQConnectionAccessor : IRabbitMQConnectionAccessor, IDisposable
    {
        public IConnection Connection { get; private set; }

        public RabbitMQConnectionAccessor(IOptions<RabbitMQOptions> options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            Connection = factory.CreateConnection();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
