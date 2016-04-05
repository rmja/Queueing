using System;

namespace Queueing
{
    public interface IMessageConverter
    {
		byte[] Serialize(object message);
        IMessage Deserialize(byte[] body, Type type);
    }

    public static class IMessageConverterExtensions
    {
        public static TMessage Deserialize<TMessage>(this IMessageConverter self, byte[] body)
            where TMessage : IMessage
        {
            return (TMessage)self.Deserialize(body, typeof(TMessage));
        }
    }
}