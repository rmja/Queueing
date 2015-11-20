using System;
using System.Threading.Tasks;

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
