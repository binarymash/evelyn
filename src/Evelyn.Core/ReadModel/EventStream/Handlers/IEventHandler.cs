namespace Evelyn.Core.ReadModel.EventStream.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEventHandler<TEventStream>
    {
        Task HandleEvent(EventEnvelope eventEnvelope, CancellationToken stoppingToken);
    }
}
