using System.Threading.Tasks;

namespace Queueing
{
    public interface IRemoteProcedureCallClient
    {
        Task<TReply> CallAsync<TReply>(IExchange exchange, IMessage request, string route)
            where TReply : IMessage;
    }
}
