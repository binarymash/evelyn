namespace Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem
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
            var evelyn = await Session.Get<Evelyn>(Constants.EvelynSystem);
            evelyn.StartSystem(message.UserId);
        }
    }
}
