namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
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
            try
            {
                var projection = new AccountProjectsDto(@event.Id, @event.Version, @event.OccurredAt, @event.UserId, @event.OccurredAt, @event.UserId, new List<ProjectListDto>());

                await Projections.AddOrUpdate(AccountProjectsDto.StoreKey(@event.Id), projection);
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
                var storeKey = AccountProjectsDto.StoreKey(@event.Id);
                var projection = await Projections.Get(storeKey).ConfigureAwait(false);

                projection.AddProject(@event.ProjectId, string.Empty, @event.Version, @event.OccurredAt, @event.UserId);

                await Projections.AddOrUpdate(storeKey, projection);
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
                var storeKey = AccountProjectsDto.StoreKey(@event.AccountId);
                var projection = await Projections.Get(storeKey).ConfigureAwait(false);

                var project = projection.Projects.Single(p => p.Id == @event.Id);
                project.SetName(@event.Name);

                await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
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
                var storeKey = AccountProjectsDto.StoreKey(@event.Id);
                var projection = await Projections.Get(storeKey).ConfigureAwait(false);

                projection.DeleteProject(@event.ProjectId, @event.Version, @event.OccurredAt, @event.UserId);

                await Projections.AddOrUpdate(storeKey, projection).ConfigureAwait(false);
            }
            catch
            {
                throw new FailedToBuildProjectionException();
            }
        }
    }
}
