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
            try
            {
                await Session.Get<Evelyn>(Constants.EvelynSystem);
                throw new ConcurrencyException(Constants.EvelynSystem);
            }
            catch (AggregateNotFoundException)
            {
                await Session.Add(new Evelyn(Constants.SystemUser, Constants.EvelynSystem));
            }
        }
    }
}
