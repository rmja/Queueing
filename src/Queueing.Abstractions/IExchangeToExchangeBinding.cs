using System;

namespace Queueing
{
    public interface IExchangeToExchangeBinding
    {
		IExchange Source { get; }
		IExchange Destination { get; }
		string Route { get; }
    }
}