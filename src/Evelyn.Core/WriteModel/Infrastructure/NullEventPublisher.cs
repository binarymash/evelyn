namespace Evelyn.Core.WriteModel.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public class NullEventPublisher : IEventPublisher
    {
        Task IEventPublisher.Publish<T>(T @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
