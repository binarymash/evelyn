namespace Evelyn.Core.WriteModel.Project.Commands.DeleteProject
{
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using Domain;

    public class Handler : Handler<Command>
    {
        public Handler(ISession session)
            : base(session, new Validator())
        {
        }

        protected override async Task HandleImpl(Command message)
        {
            var project = await Session.Get<Project>(message.ProjectId);
            project.DeleteProject(message.UserId, message.ExpectedProjectVersion);
        }
    }
}
