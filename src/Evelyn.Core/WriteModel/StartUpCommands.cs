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
        private readonly ICommandHandler<Evelyn.Commands.CreateSystem.Command> _createSystemHandler;
        private readonly ICommandHandler<Evelyn.Commands.StartSystem.Command> _startSystemHandler;

        public StartUpCommands(
            ICommandHandler<Evelyn.Commands.CreateSystem.Command> createSystemHandler,
            ICommandHandler<Evelyn.Commands.StartSystem.Command> startSystemHandler)
        {
            _createSystemHandler = createSystemHandler;
            _startSystemHandler = startSystemHandler;
        }

        public async Task Execute()
        {
            await _createSystemHandler.Handle(new Evelyn.Commands.CreateSystem.Command(Constants.SystemUser, Constants.EvelynSystem));
            await _startSystemHandler.Handle(new Evelyn.Commands.StartSystem.Command(Constants.SystemUser, Constants.EvelynSystem));
        }
    }
}