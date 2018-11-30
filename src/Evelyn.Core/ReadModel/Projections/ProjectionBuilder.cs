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

        protected EventAuditDto CreateEventAudit(Event @event, long? versionOverride = null)
        {
            return EventAuditDto.Create(@event.OccurredAt, @event.UserId, versionOverride ?? @event.Version);
        }
    }
}
