namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.WriteModel.Account.Events;
    using Infrastructure;

    public class ProjectionBuilder : ProjectionBuilder<AccountProjectsDto>
    {
        public ProjectionBuilder(IProjectionStore<AccountProjectsDto> projectionStore)
            : base(projectionStore)
        {
        }

        public override async Task HandleEvent(IEvent @event)
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
            var projection = new AccountProjectsDto(@event.Id, @event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, new List<ProjectListDto>());

            await Projections.AddOrUpdate(AccountProjectsDto.StoreKey(@event.Id), projection);
        }

        private async Task Handle(ProjectCreated @event)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);
            projection.AddProject(@event.ProjectId, string.Empty, @event.Version, @event.OccurredAt, @event.UserId);

            await Projections.AddOrUpdate(storeKey, projection);
        }

        private async Task Handle(WriteModel.Project.Events.ProjectCreated @event)
        {
                var storeKey = AccountProjectsDto.StoreKey(@event.AccountId);
                var projection = await Projections.Get(storeKey).ConfigureAwait(false);
                var project = projection.Projects.Single(p => p.Id == @event.Id);
                project.SetName(@event.Name);

                await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }

        private async Task Handle(ProjectDeleted @event)
        {
            var storeKey = AccountProjectsDto.StoreKey(@event.Id);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);

            projection.DeleteProject(@event.ProjectId, @event.Version, @event.OccurredAt, @event.UserId);

            await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
        }
    }
}
