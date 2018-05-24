namespace Evelyn.Core.WriteModel.Evelyn.Commands.CreateSystem
{
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using Domain;

    public class Handler : Handler<Command>
    {
        public Handler(ISession session)
            : base(session, new Validator())
        {
        }

        protected override async Task HandleImpl(Command message)
        {
            Evelyn evelyn;
            try
            {
                evelyn = await Session.Get<Evelyn>(Constants.EvelynSystem);
            }
            catch (AggregateNotFoundException)
            {
                evelyn = new Evelyn(Constants.SystemUser, Constants.EvelynSystem);
                await Session.Add(evelyn);

                var defaultAccount = evelyn.RegisterAccount(Constants.SystemUser, Constants.DefaultAccount);
                await Session.Add(defaultAccount);
            }
        }
    }
}
