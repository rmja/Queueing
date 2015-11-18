using System.Threading.Tasks;

namespace Queueing
{
    public static class RemoteProcedureCallExtensions
    {
        private static readonly IExchange _defaultExchange = new DefaultExchange();

        public static Task<TReply> CallAsync<TReply>(this IRemoteProcedureCallClient self, IQueue queue, IMessage request)
            where TReply : IMessage
        {
            return self.CallAsync<TReply>(_defaultExchange, request, queue.Name);
        }
    }
}
