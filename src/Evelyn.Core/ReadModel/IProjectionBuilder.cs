namespace Evelyn.Core.ReadModel
{
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public interface IProjectionBuilder
    {
        Task HandleEvent(IEvent @event);
    }
}
