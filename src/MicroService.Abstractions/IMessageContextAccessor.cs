using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    public interface IMessageContextAccessor
    {
        MessageContext MessageContext { get; set; }
    }
}
