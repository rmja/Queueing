namespace Queueing
{
    public interface IPublisher
	{
		void Publish(IExchange exchange, IMessage message, string route);
        void PublishRaw(IExchange exchange, byte[] body, string route);
    }
}