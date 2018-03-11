namespace Evelyn.Core.WriteModel.Evelyn
{
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using Domain;

    public class EvelynCommandHandler :
        ICommandHandler<CreateSystem>,
        ICommandHandler<StartSystem>,
        ICommandHandler<RegisterAccount>
    {
        private readonly ISession _session;

        public EvelynCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(CreateSystem message)
        {
            Evelyn evelyn;
            try
            {
                evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            }
            catch (AggregateNotFoundException)
            {
                evelyn = new Evelyn(Constants.SystemUser, Constants.EvelynSystem);
                await _session.Add(evelyn);

                var defaultAccount = evelyn.RegisterAccount(Constants.SystemUser, Constants.DefaultAccount);
                await _session.Add(defaultAccount);
            }

            await _session.Commit();
        }

        public async Task Handle(StartSystem message)
        {
            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            evelyn.StartSystem(message.UserId);
            await _session.Commit();
        }

        public Task Handle(RegisterAccount message)
        {
            throw new System.NotImplementedException();
        }
    }
}
