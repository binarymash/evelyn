namespace Evelyn.Core.ReadModel.ProjectDetails
{
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
        private readonly IDatabase<ProjectDetailsDto> _projectDetails;

        public ProjectDetailsHandler(IDatabase<ProjectDetailsDto> projectDetails)
        {
            _projectDetails = projectDetails;
        }

        public async Task Handle(ProjectCreated message, CancellationToken token)
        {
            await _projectDetails.Add(message.Id, new ProjectDetailsDto(message.Id, message.Name, message.Version, message.TimeStamp));
        }

        public async Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            var projectDetails = await _projectDetails.Get(message.Id);
            projectDetails.AddEnvironment(new EnvironmentListDto(message.EnvironmentId, message.Name), message.TimeStamp, message.Version);
        }

        public async Task Handle(ToggleAdded message, CancellationToken token)
        {
            var projectDetails = await _projectDetails.Get(message.Id);
            projectDetails.AddToggle(new ToggleListDto(message.ToggleId, message.Name), message.TimeStamp, message.Version);
        }
    }
}
