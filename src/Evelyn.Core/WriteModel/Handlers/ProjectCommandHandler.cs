namespace Evelyn.Core.WriteModel.Handlers
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Evelyn.Core.WriteModel.Commands;
    using Evelyn.Core.WriteModel.Domain;

    public class ProjectCommandHandler :
        ICommandHandler<CreateProject>,
        ICommandHandler<AddEnvironment>,
        ICommandHandler<AddToggle>,
        ICommandHandler<ChangeToggleState>
    {
        private readonly ISession _session;

        public ProjectCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(CreateProject message)
        {
            var project = new Project(message.UserId, message.AccountId, message.Id, message.Name);
            await _session.Add(project);
            await _session.Commit();
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
            project.AddToggle(message.UserId, message.Id, message.Name, message.Key);
            await _session.Commit();
        }

        public async Task Handle(ChangeToggleState message)
        {
            var project = await _session.Get<Project>(message.ProjectId, message.ExpectedVersion);
            project.ChangeToggleState(message.UserId, message.EnvironmentKey, message.ToggleId, message.Value);
            await _session.Commit();
        }
    }
}
