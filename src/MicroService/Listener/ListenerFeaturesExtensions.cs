using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Listener
{
    public static class ListenerFeaturesExtensions
    {
        public static AckType Ack(this IListenerFeatures self)
        {
            return self.TryGet<AckType?>("Ack") ?? AckType.Ack;
        }

        public static byte[] ReplyBody(this IListenerFeatures self)
        {
            return self.TryGet<byte[]>("ReplyBody");
        }
    }
}
