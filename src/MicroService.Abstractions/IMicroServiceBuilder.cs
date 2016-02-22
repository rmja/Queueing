using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    public interface IMicroServiceBuilder
    {
        IServiceProvider ApplicationServices { get; }

        IMicroServiceBuilder Use(Func<MicroServiceRequestDelegate, MicroServiceRequestDelegate> middleware);

        MicroServiceRequestDelegate Build();
    }
}
