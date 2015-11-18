namespace Queueing
{
    public static class PublisherExtensions
    {
        private static readonly IExchange _defaultExchange = new DefaultExchange();

        public static void Publish(this IPublisher self, IQueue queue, IMessage message)
        {
            self.Publish(_defaultExchange, message, queue.Name);
        }

        public static void Publish(this IPublisher self, IExchange exchange, IMessage message)
        {
            self.Publish(exchange, message, string.Empty);
        }
    }
}
