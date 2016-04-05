using Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Middlewares
{
    public class MicroServiceConsumerOptions
    {
        public Type HandlerType { get; set; }
        public Type MessageType { get; set; }
        public string[] Routes { get; set; }
    }
}
