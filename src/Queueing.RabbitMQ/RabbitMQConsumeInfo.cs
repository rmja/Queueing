#if DNX451
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queueing.RabbitMQ
{
    public class RabbitMQConsumeInfo : IConsumeInfo
    {
        public string Exchange
        {
            get
            {
                object header;
                if (EventArgs.BasicProperties.Headers.TryGetValue("x-death", out header))
                {
                    var headerList = header as IList<object>;
                    if (headerList != null && headerList.Count > 0)
                    {
                        object exchange;
                        var first = headerList[0] as IDictionary<string, object>;
                        if (first != null && first.TryGetValue("exchange", out exchange))
                        {
                            // Message was dead lettered.

                            var exchangeBytes = exchange as byte[];
                            var exchangeString = exchange as string;

                            if (exchangeBytes != null)
                            {
                                return Encoding.UTF8.GetString((byte[])exchange);
                            }
                            else if (exchangeString != null)
                            {
                                return exchangeString;
                            }
                        }
                    }
                }
                return EventArgs.Exchange;
            }
        }
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