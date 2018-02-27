namespace Evelyn.Core.WriteModel.Handlers
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Evelyn.Core.WriteModel.Commands;
    using Evelyn.Core.WriteModel.Domain;

    public class ApplicationCommandHandler :
        ICommandHandler<CreateApplication>,
        ICommandHandler<AddEnvironment>,
        ICommandHandler<AddToggle>,
        ICommandHandler<ChangeToggleState>
    {
        private readonly ISession _session;

        public ApplicationCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(CreateApplication message)
        {
            var application = new Application(message.Id, message.Name);
            await _session.Add(application);
            await _session.Commit();
        }

        public async Task Handle(AddEnvironment message)
        {
            var application = await _session.Get<Application>(message.ApplicationId, message.ExpectedVersion);
            application.AddEnvironment(message.Id, message.Name);
            await _session.Commit();
        }

        public async Task Handle(AddToggle message)
        {
            var application = await _session.Get<Application>(message.ApplicationId, message.ExpectedVersion);
            application.AddToggle(message.Id, message.Name, message.Key);
            await _session.Commit();
        }

        public async Task Handle(ChangeToggleState message)
        {
            var application = await _session.Get<Application>(message.ApplicationId, message.ExpectedVersion);
            application.ChangeToggleState(message.EnvironmentId, message.ToggleId, message.Value);
            await _session.Commit();
        }
    }
}
