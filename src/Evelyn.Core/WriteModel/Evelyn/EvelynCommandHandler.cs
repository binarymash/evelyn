namespace Evelyn.Core.WriteModel.Evelyn
{
    using System.Threading.Tasks;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Domain;

    public class EvelynCommandHandler :
        ICommandHandler<StartSystem>,
        ICommandHandler<RegisterAccount>
    {
        private readonly ISession _session;

        public EvelynCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(StartSystem message)
        {
            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            if (evelyn == null)
            {
                evelyn = new Evelyn(Constants.SystemUser, Constants.EvelynSystem);
                await _session.Add(evelyn);

                var account = evelyn.RegisterAccount(Constants.SystemUser, Constants.DefaultAccount);
                await _session.Add(account);
            }

            evelyn.StartSystem(message.UserId);
            await _session.Commit();
        }

        public Task Handle(RegisterAccount message)
        {
            throw new System.NotImplementedException();
        }
    }
}
