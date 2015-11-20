using System;

namespace MicroService
{
    public interface IMicroServiceEngine
    {
        IServiceProvider ApplicationServices { get; }
        IDisposable Start();
    }
}