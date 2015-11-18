using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using Queueing;

namespace MicroService.Listener
{
    public interface IListenerFeatures : IDisposable
    {
        T Get<T>(string key);
        T TryGet<T>(string key);
        void Set<T>(string key, T value);
    }
}
