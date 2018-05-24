namespace Evelyn.Core.WriteModel.Evelyn.Commands.RegisterAccount
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
            var account = evelyn.RegisterAccount(message.UserId, message.AccountId);
            await Session.Add(account);
        }
    }
}
