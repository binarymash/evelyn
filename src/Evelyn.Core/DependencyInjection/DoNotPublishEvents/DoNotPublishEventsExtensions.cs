namespace Microsoft.Extensions.DependencyInjection
{
    using CQRSlite.Events;
    using Evelyn.Core.WriteModel.Infrastructure;

    public static class DoNotPublishEventsExtensions
    {
        public static void DoNotPublishEvents(this EventPublisherRegistration parentRegistration)
        {
            parentRegistration.Services.AddSingleton<IEventPublisher, NullEventPublisher>();
        }
    }
}
