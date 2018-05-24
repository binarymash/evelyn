namespace Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem
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
            var evelyn = await Session.Get<Evelyn>(Constants.EvelynSystem);
            evelyn.StartSystem(message.UserId);
        }
    }
}
