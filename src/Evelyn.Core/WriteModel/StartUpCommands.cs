namespace Evelyn.Core.WriteModel
{
    using System.Linq;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using Evelyn.Commands;

    public class StartUpCommands : IStartUpCommands
    {
        private readonly ICommandHandler<CreateSystem> _createSystemHandler;
        private readonly ICommandHandler<StartSystem> _startSystemHandler;

        public StartUpCommands(
            ICommandHandler<CreateSystem> createSystemHandler,
            ICommandHandler<StartSystem> startSystemHandler)
        {
            _createSystemHandler = createSystemHandler;
            _startSystemHandler = startSystemHandler;
        }

        public async Task Execute()
        {
            await _createSystemHandler.Handle(new CreateSystem(Constants.SystemUser, Constants.EvelynSystem));
            await _startSystemHandler.Handle(new StartSystem(Constants.SystemUser, Constants.EvelynSystem));
        }
    }
}