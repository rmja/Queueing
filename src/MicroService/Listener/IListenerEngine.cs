using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Queueing;

namespace MicroService.Listener
{
    /// <summary>
    /// https://github.com/aspnet/Hosting/blob/dev/src/Microsoft.AspNet.Hosting.Server.Abstractions/IServerFactory.cs
    /// </summary>
    public interface IListenerEngine
    {
        IDisposable Start(Func<IListenerFeatures, Task> application);
    }
}
