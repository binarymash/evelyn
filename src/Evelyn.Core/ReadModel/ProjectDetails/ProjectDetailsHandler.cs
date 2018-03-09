namespace Evelyn.Core.ReadModel.ProjectDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class ProjectDetailsHandler :
        ICancellableEventHandler<ProjectCreated>,
        ICancellableEventHandler<EnvironmentAdded>,
        ICancellableEventHandler<ToggleAdded>
    {
        private readonly IDatabase<Guid, ProjectDetailsDto> _db;

        public ProjectDetailsHandler(IDatabase<Guid, ProjectDetailsDto> db)
        {
            _db = db;
        }

        public async Task Handle(ProjectCreated message, CancellationToken token)
        {
            await _db.AddOrUpdate(message.Id, new ProjectDetailsDto(message.Id, message.Name, message.Version, message.TimeStamp));
        }

        public async Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            var projectDetails = await _db.Get(message.Id);
            projectDetails.AddEnvironment(new EnvironmentListDto(message.Key), message.TimeStamp, message.Version);
        }

        public async Task Handle(ToggleAdded message, CancellationToken token)
        {
            var projectDetails = await _db.Get(message.Id);
            projectDetails.AddToggle(new ToggleListDto(message.Key, message.Name), message.TimeStamp, message.Version);
        }
    }
}
