using System;
using System.Threading.Tasks;

namespace Queueing
{
    public interface IConsumer
    {
        IConsumeInfo Consume(IQueue queue, TimeSpan timeout);
        void Ack(IConsumeInfo consumed);
        void Reject(IConsumeInfo consumed, bool requeue);
        void SendRepy(IConsumeInfo consumed, byte[] body);
    }
}