namespace ReactChat.Shared.Messaging.Messaging
{
    public static class RabbitMQQueues
    {
        public const string UserRegistrationQueue = "user.registration.queue";
        public const string OrderCreatedQueue = "order.created.queue";
        public const string PaymentProcessedQueue = "payment.processed.queue";
    }
}
