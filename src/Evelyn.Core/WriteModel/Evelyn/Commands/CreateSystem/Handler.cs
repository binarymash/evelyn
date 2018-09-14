namespace Evelyn.Core.WriteModel.Evelyn.Commands.CreateSystem
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
            try
            {
                var evelyn = await Session.Get<Evelyn>(Constants.EvelynSystem);
                throw new ConcurrencyException(Constants.EvelynSystem, message.ExpectedVersion.Value, evelyn.Version);
            }
            catch (CQRSlite.Domain.Exception.AggregateNotFoundException)
            {
                await Session.Add(new Evelyn(Constants.SystemUser, Constants.EvelynSystem));
            }
        }
    }
}
