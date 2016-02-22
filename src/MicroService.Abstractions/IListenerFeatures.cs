using System;

namespace MicroService.Listener
{
    public interface IListenerFeatures : IDisposable
    {
        T Get<T>(string key);
        T TryGet<T>(string key);
        void Set<T>(string key, T value);
    }
}
