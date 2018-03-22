namespace Evelyn.Core.WriteModel.Project
{
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Domain;

    public class ProjectCommandHandler :
        ICommandHandler<AddEnvironment>,
        ICommandHandler<AddToggle>,
        ICommandHandler<ChangeToggleState>
    {
        private readonly ISession _session;

        public ProjectCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(AddEnvironment message)
        {
            var project = await _session.Get<Project>(message.ProjectId, message.ExpectedVersion);
            project.AddEnvironment(message.UserId, message.Key);
            await _session.Commit();
        }

        public async Task Handle(AddToggle message)
        {
            var project = await _session.Get<Project>(message.ProjectId, message.ExpectedVersion);
            project.AddToggle(message.UserId, message.Key, message.Name);
            await _session.Commit();
        }

        public async Task Handle(ChangeToggleState message)
        {
            var project = await _session.Get<Project>(message.ProjectId);
            project.ChangeToggleState(message.UserId, message.EnvironmentKey, message.ToggleKey, message.Value, message.ExpectedVersion);
            await _session.Commit();
        }
    }
}
