namespace Evelyn.Core.WriteModel.Evelyn.Commands.RegisterAccount
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
            var account = evelyn.RegisterAccount(message.UserId, message.AccountId);
            await Session.Add(account);
        }
    }
}
