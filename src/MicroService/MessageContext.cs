using MicroService.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService
{
    public class MessageContext
    {
        private readonly IListenerFeatures _features;

        public IServiceProvider ApplicationServices => _features.Get<IServiceProvider>("ApplicationServices");
        public IServiceProvider RequestServices => _features.Get<IServiceProvider>("RequestServices");
        public string QueueName => _features.Get<string>("QueueName");
        public string Route => _features.Get<string>("Route");
        public byte[] Body => _features.Get<byte[]>("Body");

        public AckType Ack { get { return _features.Get<AckType>("Ack"); } set { _features.Set("Ack", value) ; } }
        public byte[] ReplyBody { get { return _features.Get<byte[]>("ReplyBody"); } set { _features.Set("ReplyBody", value); } }

        public MessageContext(IListenerFeatures features)
        {
            _features = features;
        }
    }

    public enum AckType
    {
        Ack,
        Reject,
        RejectButRequeue
    }
}
