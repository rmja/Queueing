using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Listener
{
    public static class ListenerFeaturesExtensions
    {
        public static AckType GetAck(this IListenerFeatures self)
        {
            return self.TryGet<AckType?>("Ack") ?? AckType.Ack;
        }

        public static byte[] TryGetReplyBody(this IListenerFeatures self)
        {
            return self.TryGet<byte[]>("ReplyBody");
        }
    }
}
