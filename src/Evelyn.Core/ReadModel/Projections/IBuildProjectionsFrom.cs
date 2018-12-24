namespace Evelyn.Core.ReadModel.Projections
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public interface IBuildProjectionsFrom<TEvent>
        where TEvent : IEvent
    {
        Task Handle(long streamPosition, TEvent @event, CancellationToken stoppingToken);
    }
}
