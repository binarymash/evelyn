namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.WriteModel.Project.Events;
    using Infrastructure;

    public class EventStreamHandler : EventStreamHandler<ProjectDetailsDto>
    {
        private IProjectionStore<Guid, ProjectDetailsDto> _db;

        public EventStreamHandler(
            IProjectionStore<Guid, ProjectDetailsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(eventQueueFactory)
        {
            _db = db;
        }

        protected override async Task HandleEvent(IEvent @event)
        {
            switch (@event)
            {
                case ProjectCreated projectCreated:
                    await Handle(projectCreated).ConfigureAwait(false);
                    break;
                case EnvironmentAdded environmentAdded:
                    await Handle(environmentAdded).ConfigureAwait(false);
                    break;
                case EnvironmentDeleted environmentDeleted:
                    await Handle(environmentDeleted).ConfigureAwait(false);
                    break;
                case ToggleAdded toggleAdded:
                    await Handle(toggleAdded).ConfigureAwait(false);
                    break;
                case ToggleDeleted toggleDeleted:
                    await Handle(toggleDeleted).ConfigureAwait(false);
                    break;
                case ProjectDeleted projectDeleted:
                    await Handle(projectDeleted).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task Handle(ProjectCreated @event)
        {
            try
            {
                var projection = new ProjectDetailsDto(@event.Id, @event.Name, null, null, @event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(@event.Id, projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(EnvironmentAdded @event)
        {
            try
            {
                var projection = await _db.Get(@event.Id).ConfigureAwait(false);
                projection.AddEnvironment(@event.Key, @event.Name, @event.OccurredAt, @event.Version, @event.UserId);
                await _db.AddOrUpdate(@event.Id, projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(EnvironmentDeleted @event)
        {
            try
            {
                var projection = await _db.Get(@event.Id).ConfigureAwait(false);
                projection.DeleteEnvironment(@event.Key, @event.OccurredAt, @event.UserId, @event.Version);
                await _db.AddOrUpdate(@event.Id, projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ToggleAdded @event)
        {
            try
            {
                var projection = await _db.Get(@event.Id).ConfigureAwait(false);
                projection.AddToggle(@event.Key, @event.Name, @event.OccurredAt, @event.UserId, @event.Version);
                await _db.AddOrUpdate(@event.Id, projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ToggleDeleted @event)
        {
            try
            {
                var projection = await _db.Get(@event.Id).ConfigureAwait(false);
                projection.DeleteToggle(@event.Key, @event.OccurredAt, @event.UserId, @event.Version);
                await _db.AddOrUpdate(@event.Id, projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ProjectDeleted @event)
        {
            try
            {
                await _db.Delete(@event.Id);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
