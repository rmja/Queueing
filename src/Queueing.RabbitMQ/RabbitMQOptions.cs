using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queueing.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string Hostname { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
