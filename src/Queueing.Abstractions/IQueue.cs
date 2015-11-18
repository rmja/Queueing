using System;
using System.Collections.Generic;

namespace Queueing
{
    public interface IQueue
    {
		string Name { get; }
		IExchange DeadLetterExchange { get; }
	}
}