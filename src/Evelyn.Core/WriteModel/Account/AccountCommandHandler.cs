namespace Evelyn.Core.WriteModel.Account
{
    using System.Threading.Tasks;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;

    public class AccountCommandHandler :
        ICommandHandler<CreateProject>
    {
        private readonly ISession _session;

        public AccountCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(CreateProject message)
        {
            var account = await _session.Get<Domain.Account>(message.Id, message.ExpectedVersion);
            var project = account.CreateProject(message.UserId, message.ProjectId, message.Name);
            await _session.Add(project);
            await _session.Commit();
        }
    }
}
