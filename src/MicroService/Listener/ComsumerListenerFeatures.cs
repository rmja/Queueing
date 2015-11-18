using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Listener
{
    public class ComsumerListenerFeatures : IListenerFeatures
    {
        private readonly IDictionary<string, object> _features;

        public ComsumerListenerFeatures(IDictionary<string, object> features)
        {
            _features = features;
        }

        public T Get<T>(string key)
        {
            return (T)_features[key];
        }

        public T TryGet<T>(string key)
        {
            return _features.ContainsKey(key) ? (T)_features[key] : default(T);
        }

        public void Set<T>(string key, T value)
        {
            _features[key] = value;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
