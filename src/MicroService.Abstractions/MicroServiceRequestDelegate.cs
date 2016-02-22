using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    public delegate Task MicroServiceRequestDelegate(MessageContext context);
}
