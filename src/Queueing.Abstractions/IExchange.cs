using System;

namespace Queueing
{
    public interface IExchange
    {
		string Name { get; }
		IExchange AlternateExchange { get; }
	}
}