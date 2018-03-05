namespace Evelyn.Core.ReadModel.ProjectList
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Events;
    using Infrastructure;

    public class ProjectListHandler
        : ICancellableEventHandler<ProjectCreated>
    {
        private readonly IDatabase<ProjectListDto> _projects;

        public ProjectListHandler(IDatabase<ProjectListDto> projects)
        {
            _projects = projects;
        }

        public Task Handle(ProjectCreated message, CancellationToken token)
        {
            _projects.Add(message.Id, new ProjectListDto(message.Id, message.Name));
            return Task.CompletedTask;
        }
    }
}
