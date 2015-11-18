using System;
using System.Threading.Tasks;

namespace Queueing
{
    public interface IPublisher
	{
		void Publish(IExchange exchange, IMessage message, string route);
    }
}