namespace Evelyn.Core.WriteModel.Project.Commands.AddEnvironment
{
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using Domain;
    using Microsoft.Extensions.Logging;

    public class Handler : Handler<Command>
    {
        public Handler(ILogger<Command> logger, ISession session)
            : base(logger, session, new Validator())
        {
        }

        protected override async Task HandleImpl(Command message)
        {
            var project = await Session.Get<Project>(message.ProjectId);
            project.AddEnvironment(message.UserId, message.Key, message.Name, message.ExpectedProjectVersion);
        }
    }
}
