namespace Evelyn.Core.WriteModel.Account.Commands.CreateProject
{
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using Microsoft.Extensions.Logging;

    public class Handler : Handler<Command>
    {
        public Handler(ILogger<Command> logger, ISession session)
            : base(logger, session, new Validator())
        {
        }

        protected override async Task HandleImpl(Command message)
        {
            var account = await Session.Get<Domain.Account>(message.Id).ConfigureAwait(false);
            var project = account.CreateProject(message.UserId, message.ProjectId, message.Name, message.ExpectedVersion);
            await Session.Add(project).ConfigureAwait(false);
        }
    }
}
