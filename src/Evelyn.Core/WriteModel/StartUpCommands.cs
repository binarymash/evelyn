namespace Evelyn.Core.WriteModel
{
    using System.Linq;
    using System.Threading.Tasks;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;

    public class StartUpCommands : IStartUpCommands
    {
        private readonly ISession _session;

        public StartUpCommands(ISession session)
        {
            _session = session;
        }

        public async Task Execute()
        {
            await EnsureSystemIsInitialised();
        }

        private async Task EnsureSystemIsInitialised()
        {
            Evelyn.Domain.Evelyn evelyn;
            try
            {
                evelyn = await _session.Get<Evelyn.Domain.Evelyn>(Constants.EvelynSystem);
            }
            catch (AggregateNotFoundException)
            {
                evelyn = new Evelyn.Domain.Evelyn(Constants.SystemUser, Constants.EvelynSystem);
                await _session.Add(evelyn);
            }

            if (!evelyn.Accounts.Any())
            {
                var defaultAccount = evelyn.RegisterAccount(Constants.SystemUser, Constants.DefaultAccount);
                await _session.Add(defaultAccount);
            }

            evelyn.StartSystem(Constants.SystemUser);
            await _session.Commit();
        }
    }
}