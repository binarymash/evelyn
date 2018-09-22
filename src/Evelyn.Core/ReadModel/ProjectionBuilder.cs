namespace Evelyn.Core.ReadModel
{
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public abstract class ProjectionBuilder<TDto> : IProjectionBuilder
    {
        public ProjectionBuilder(IProjectionStore<TDto> projections)
        {
            Projections = projections;
        }

        protected IProjectionStore<TDto> Projections { get; }

        public abstract Task HandleEvent(IEvent @event);
    }
}
