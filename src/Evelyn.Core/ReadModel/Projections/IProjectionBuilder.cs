namespace Evelyn.Core.ReadModel.Projections
{
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public interface IProjectionBuilder
    {
        Task HandleEvent(IEvent @event);
    }
}
