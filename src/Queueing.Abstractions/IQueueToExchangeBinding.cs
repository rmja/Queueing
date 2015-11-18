using System;

namespace Queueing
{
    public interface IQueueToExchangeBinding
    {
		IQueue Source { get; }
		IExchange Destination { get; }
		string Route { get; }
    }
}