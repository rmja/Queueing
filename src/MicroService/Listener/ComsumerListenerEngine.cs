using Microsoft.Framework.DependencyInjection;
using Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Configuration;
using System.Threading;

namespace MicroService.Listener
{
    /// <summary>
    /// https://github.com/aspnet/KestrelHttpServer/blob/dev/src/Kestrel/ServerFactory.cs
    /// </summary>
    public class ComsumerListenerEngine<TQueue> : IListenerEngine, IDisposable
        where TQueue : IQueue, new()
    {
        private readonly IConsumer _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _applicationServices;
        private Task _task;
        private bool _running;

        private static TQueue _queue = new TQueue();

        public ComsumerListenerEngine(IConsumer consumer, IServiceScopeFactory serviceScopeFactory, IServiceProvider applicationServices)
        {
            _consumer = consumer;
            _serviceScopeFactory = serviceScopeFactory;
            _applicationServices = applicationServices;
        }

        public IDisposable Start(Func<IListenerFeatures, Task> application)
        {
            _running = true;
            _task = Task.Run(async () => {

                while (_running)
                {
                    var consumed = _consumer.Consume(_queue, TimeSpan.FromMilliseconds(500));

                    if (consumed != null)
                    {
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var features = new ComsumerListenerFeatures(new Dictionary<string, object>()
                                {
                                    { "ApplicationServices", _applicationServices },
                                    { "RequestServices", scope.ServiceProvider },
                                    { "Route", consumed.Route },
                                    { "Body", consumed.Body }
                                });

                            await application(features);
                            
                            var replyBody = features.ReplyBody();
                            if (replyBody != null)
                            {
                                _consumer.SendRepy(consumed, replyBody);
                            }

                            switch (features.Ack())
                            {
                                case AckType.Ack:
                                    _consumer.Ack(consumed);
                                    break;
                                case AckType.RejectButRequeue:
                                    _consumer.Reject(consumed, true);
                                    break;
                                case AckType.Reject:
                                    _consumer.Reject(consumed, false);
                                    break;
                            }
                        }
                    }
                }
            });

            return this;
        }

        public void Dispose()
        {
            _running = false;
            _task.Wait();
        }
    }
}
