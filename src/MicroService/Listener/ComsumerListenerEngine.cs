using Microsoft.Extensions.DependencyInjection;
using Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace MicroService.Listener
{
    /// <summary>
    /// https://github.com/aspnet/KestrelHttpServer/blob/dev/src/Kestrel/ServerFactory.cs
    /// </summary>
    public class ComsumerListenerEngine<TQueue> : IListenerEngine, IDisposable
        where TQueue : IQueue, new()
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _applicationServices;
        private Task _task;
        private bool _running;

        private static TQueue _queue = new TQueue();

        public ComsumerListenerEngine(IServiceScopeFactory serviceScopeFactory, IServiceProvider applicationServices)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _applicationServices = applicationServices;
        }

        public IDisposable Start(Func<IListenerFeatures, Task> application)
        {
            _running = true;
            _task = Task.Run(async () => {
                Console.WriteLine($"Listinging on {_queue.Name}");
                while (_running)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();
                        var consumed = consumer.Consume(_queue, TimeSpan.FromMilliseconds(1000));

                        if (consumed != null)
                        {
                            var features = new ComsumerListenerFeatures(new Dictionary<string, object>()
                            {
                                { "ApplicationServices", _applicationServices },
                                { "RequestServices", scope.ServiceProvider },
                                { "Route", consumed.Route },
                                { "Body", consumed.Body }
                            });

                            try
                            {
                                await application(features);

                                var replyBody = features.ReplyBody();
                                if (replyBody != null)
                                {
                                    consumer.SendRepy(consumed, replyBody);
                                }

                                switch (features.Ack())
                                {
                                    case AckType.Ack:
                                        consumer.Ack(consumed);
                                        break;
                                    case AckType.RejectButRequeue:
                                        consumer.Reject(consumed, true);
                                        break;
                                    case AckType.Reject:
                                        consumer.Reject(consumed, false);
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Application error: '{ex.Message}'.");
                                consumer.Reject(consumed, false);
                            }
                        }
                    }
                }
                Console.WriteLine($"Stopped listening to {_queue.Name}.");
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
