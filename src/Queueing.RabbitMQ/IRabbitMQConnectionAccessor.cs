using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queueing.RabbitMQ
{
    public interface IRabbitMQConnectionAccessor
    {
        IConnection Connection { get; }
    }
}
