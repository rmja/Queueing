#if DNX451
using RabbitMQ.Client.Events;

namespace Queueing.RabbitMQ
{
    public class RabbitMQConsumeInfo : IConsumeInfo
    {
        public string Route => EventArgs.RoutingKey;
        public byte[] Body => EventArgs.Body;

        public BasicDeliverEventArgs EventArgs { get; private set; }

        public RabbitMQConsumeInfo(BasicDeliverEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }
    }
}
#endif