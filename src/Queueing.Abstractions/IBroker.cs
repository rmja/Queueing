using System;

namespace Queueing
{
    public interface IBroker
    {
		void DeclareExchange(IExchange exchange);
		void DeclareQueue(IQueue queue);
		void Bind(IQueue destination, IExchange source, params string[] topics);
		void Bind(IExchange destination, IExchange source, params string[] topics);
	}
}