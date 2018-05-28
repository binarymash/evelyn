namespace Evelyn.Core.WriteModel
{
    using System.IO.Compression;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain.Exception;

    public class StartUpCommands : IStartUpCommands
    {
        private readonly ICommandHandler<Evelyn.Commands.CreateSystem.Command> _createSystemHandler;
        private readonly ICommandHandler<Evelyn.Commands.RegisterAccount.Command> _registerAccountHandler;
        private readonly ICommandHandler<Evelyn.Commands.StartSystem.Command> _startSystemHandler;
        private readonly ICommandHandler<Account.Commands.CreateProject.Command> _createProjectHandler;
        private readonly ICommandHandler<Project.Commands.AddEnvironment.Command> _addEnvironmentHandler;
        private readonly ICommandHandler<Project.Commands.AddToggle.Command> _addToggleHandler;

        public StartUpCommands(
            ICommandHandler<Evelyn.Commands.CreateSystem.Command> createSystemHandler,
            ICommandHandler<Evelyn.Commands.RegisterAccount.Command> registerAccountHandler,
            ICommandHandler<Evelyn.Commands.StartSystem.Command> startSystemHandler,
            ICommandHandler<Account.Commands.CreateProject.Command> createProjectHandler,
            ICommandHandler<Project.Commands.AddEnvironment.Command> addEnvironmentHandler,
            ICommandHandler<Project.Commands.AddToggle.Command> addToggleHandler)
        {
            _createSystemHandler = createSystemHandler;
            _registerAccountHandler = registerAccountHandler;
            _startSystemHandler = startSystemHandler;
            _createProjectHandler = createProjectHandler;
            _addEnvironmentHandler = addEnvironmentHandler;
            _addToggleHandler = addToggleHandler;
        }

        public async Task Execute()
        {
            var systemCreated = false;

            try
            {
                await _createSystemHandler.Handle(new Evelyn.Commands.CreateSystem.Command(Constants.SystemUser, Constants.EvelynSystem));
                systemCreated = true;
            }
            catch (ConcurrencyException)
            {
                // System was already created
                // TODO: I don't like throwing an exception every time we start up. Is there a better way of doing this?
            }

            await _startSystemHandler.Handle(new Evelyn.Commands.StartSystem.Command(Constants.SystemUser, Constants.EvelynSystem));

            if (systemCreated)
            {
                await _registerAccountHandler.Handle(new Evelyn.Commands.RegisterAccount.Command(Constants.SystemUser, Constants.DefaultAccount));
                await _createProjectHandler.Handle(new Account.Commands.CreateProject.Command(Constants.SystemUser, Constants.DefaultAccount, Constants.SampleProject, "My First Project"));
                await _addEnvironmentHandler.Handle(new Project.Commands.AddEnvironment.Command(Constants.SystemUser, Constants.SampleProject, "my-first-environment", 0));
                await _addToggleHandler.Handle(new Project.Commands.AddToggle.Command(Constants.SystemUser, Constants.SampleProject, "my-first-toggle", "My First Toggle", 1));
            }
        }
    }
}