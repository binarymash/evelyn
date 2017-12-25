// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using CQRSlite.Events;
    using Evelyn.Core.WriteModel.Infrastructure;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class DoNotPublishEventsExtensions
    {
        public static void DoNotPublishEvents(this EventPublisherRegistration parentRegistration)
        {
            parentRegistration.Services.TryAddSingleton<IEventPublisher, NullEventPublisher>();
        }
    }
}
