namespace Evelyn.Core.ReadModel.Projections
{
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Evelyn.Core.WriteModel;

    public abstract class ProjectionBuilder<TDto>
    {
        public ProjectionBuilder(IProjectionStore<TDto> projections)
        {
            Projections = projections;
        }

        protected IProjectionStore<TDto> Projections { get; }

        protected EventAuditDto CreateEventAudit(long streamVersion, Event @event, long? aggregateRootVersionOverride = null)
        {
            return EventAuditDto.Create(@event.OccurredAt, @event.UserId, aggregateRootVersionOverride ?? @event.Version, streamVersion);
        }
    }
}
