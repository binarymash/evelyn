namespace Evelyn.Core.WriteModel.Account.Commands.CreateProject
{
    using System.Threading.Tasks;
    using CQRSlite.Domain;

    public class Handler : Handler<Command>
    {
        public Handler(ISession session)
            : base(session, new Validator())
        {
        }

        protected override async Task HandleImpl(Command message)
        {
            var account = await Session.Get<Domain.Account>(message.Id, message.ExpectedVersion).ConfigureAwait(false);
            var project = account.CreateProject(message.UserId, message.ProjectId, message.Name);
            await Session.Add(project).ConfigureAwait(false);
        }
    }
}
