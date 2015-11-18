using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    /// <summary>
    /// https://github.com/aspnet/HttpAbstractions/blob/dev/src/Microsoft.AspNet.Http/ApplicationBuilder.cs
    /// </summary>
    public class MicroServiceBuilder : IMicroServiceBuilder
    {
        private readonly IList<Func<MicroServiceRequestDelegate, MicroServiceRequestDelegate>> _components = new List<Func<MicroServiceRequestDelegate, MicroServiceRequestDelegate>>();

        public IServiceProvider ApplicationServices { get; }

        public MicroServiceBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
        }

        //public void Use<TMiddleware>(params object[] args)
        public IMicroServiceBuilder Use(Func<MicroServiceRequestDelegate, MicroServiceRequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public MicroServiceRequestDelegate Build()
        {
            MicroServiceRequestDelegate app = context =>
            {
                throw new Exception(string.Format("No middleware registered for route '{0}'.", context.Route));
            };

            foreach (var component in _components)
            {
                app = component(app);
            }

            return app;
        }
    }
}
