namespace Evelyn.Core.WriteModel.Project
{
    using System.Threading.Tasks;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Domain;

    public class ProjectCommandHandler :
        ICommandHandler<AddEnvironment>,
        ICommandHandler<AddToggle>,
        ICommandHandler<ChangeToggleState>,
        ICommandHandler<DeleteToggle>,
        ICommandHandler<DeleteEnvironment>
    {
        private readonly ISession _session;

        public ProjectCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(AddEnvironment message)
        {
            var project = await _session.Get<Project>(message.ProjectId);
            project.AddEnvironment(message.UserId, message.Key, message.ExpectedProjectVersion);
            await _session.Commit();
        }

        public async Task Handle(AddToggle message)
        {
            var project = await _session.Get<Project>(message.ProjectId);
            project.AddToggle(message.UserId, message.Key, message.Name, message.ExpectedProjectVersion);
            await _session.Commit();
        }

        public async Task Handle(ChangeToggleState message)
        {
            var project = await _session.Get<Project>(message.ProjectId);
            project.ChangeToggleState(message.UserId, message.EnvironmentKey, message.ToggleKey, message.Value, message.ExpectedToggleStateVersion);
            await _session.Commit();
        }

        public async Task Handle(DeleteToggle message)
        {
            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteToggle(message.UserId, message.Key, message.ExpectedToggleVersion);
            await _session.Commit();
        }

        public async Task Handle(DeleteEnvironment message)
        {
            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteEnvironment(message.UserId, message.Key, message.ExpectedEnvironmentVersion);
            await _session.Commit();
        }
    }
}
