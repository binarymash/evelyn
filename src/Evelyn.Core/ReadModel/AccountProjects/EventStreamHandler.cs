namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.WriteModel.Account.Events;
    using Infrastructure;

    public class EventStreamHandler : EventStreamHandler<AccountProjectsDto>
    {
        private readonly IProjectionStore<Guid, AccountProjectsDto> _db;

        public EventStreamHandler(
            IProjectionStore<Guid, AccountProjectsDto> db,
            IEventStreamFactory eventQueueFactory)
            : base(eventQueueFactory)
        {
            _db = db;
        }

        protected override async Task HandleEvent(IEvent @event)
        {
            switch (@event)
            {
                case AccountRegistered accountRegistered:
                    await Handle(accountRegistered).ConfigureAwait(false);
                    break;
                case ProjectCreated projectCreated:
                    await Handle(projectCreated).ConfigureAwait(false);
                    break;
                case ProjectDeleted projectDeleted:
                    await Handle(projectDeleted).ConfigureAwait(false);
                    break;
                case WriteModel.Project.Events.ProjectCreated projectCreated:
                    await Handle(projectCreated).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task Handle(AccountRegistered @event)
        {
            try
            {
                var dto = new AccountProjectsDto(@event.Id, @event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, new List<ProjectListDto>());
                await _db.AddOrUpdate(dto.AccountId, dto);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(ProjectCreated @event)
        {
            try
            {
                var dto = await _db.Get(@event.Id).ConfigureAwait(false);
                dto.AddProject(@event.ProjectId, string.Empty, @event.Version, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(dto.AccountId, dto);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }

        private async Task Handle(WriteModel.Project.Events.ProjectCreated @event)
        {
            try
            {
                var dto = await _db.Get(@event.AccountId).ConfigureAwait(false);

                var project = dto.Projects.Single(p => p.Id == @event.Id);
                project.SetName(@event.Name);
                await _db.AddOrUpdate(dto.AccountId, dto);
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
                var dto = await _db.Get(@event.Id).ConfigureAwait(false);
                dto.DeleteProject(@event.ProjectId, @event.Version, @event.OccurredAt, @event.UserId);
                await _db.AddOrUpdate(dto.AccountId, dto);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
