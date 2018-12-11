namespace Evelyn.Core.ReadModel.Projections
{
    using Evelyn.Core.WriteModel;

    public abstract class ProjectionBuilder<TDto>
    {
        public ProjectionBuilder(IProjectionStore<TDto> projections)
        {
            Projections = projections;
        }

        protected IProjectionStore<TDto> Projections { get; }

        protected EventAudit CreateEventAudit(long streamVersion, Event @event)
        {
            return EventAudit.Create(@event.OccurredAt, @event.UserId, @event.Version, streamVersion);
        }
    }
}
