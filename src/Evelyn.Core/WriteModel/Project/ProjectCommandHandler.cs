namespace Evelyn.Core.WriteModel.Project
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Domain;
    using FluentValidation;

    public class ProjectCommandHandler :
        ICommandHandler<Commands.AddEnvironment.Command>,
        ICommandHandler<Commands.AddToggle.Command>,
        ICommandHandler<Commands.ChangeToggleState.Command>,
        ICommandHandler<Commands.DeleteToggle.Command>,
        ICommandHandler<Commands.DeleteEnvironment.Command>
    {
        private readonly ISession _session;
        private readonly AbstractValidator<Commands.AddEnvironment.Command> _addEnvironmentValidator;
        private readonly AbstractValidator<Commands.AddToggle.Command> _addToggleValidator;
        private readonly AbstractValidator<Commands.ChangeToggleState.Command> _changeToggleStateValidator;
        private readonly AbstractValidator<Commands.DeleteToggle.Command> _deleteToggleValidator;
        private readonly AbstractValidator<Commands.DeleteEnvironment.Command> _deleteEnvironmentValidator;

        public ProjectCommandHandler(ISession session)
        {
            _session = session;
            _addEnvironmentValidator = new Commands.AddEnvironment.Validator();
            _addToggleValidator = new Commands.AddToggle.Validator();
            _changeToggleStateValidator = new Commands.ChangeToggleState.Validator();
            _deleteToggleValidator = new Commands.DeleteToggle.Validator();
            _deleteEnvironmentValidator = new Commands.DeleteEnvironment.Validator();
        }

        public async Task Handle(Commands.AddEnvironment.Command message)
        {
            await _addEnvironmentValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.AddEnvironment(message.UserId, message.Key, message.ExpectedProjectVersion);
            await _session.Commit();
        }

        public async Task Handle(Commands.AddToggle.Command message)
        {
            await _addToggleValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.AddToggle(message.UserId, message.Key, message.Name, message.ExpectedProjectVersion);
            await _session.Commit();
        }

        public async Task Handle(Commands.ChangeToggleState.Command message)
        {
            await _changeToggleStateValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.ChangeToggleState(message.UserId, message.EnvironmentKey, message.ToggleKey, message.Value, message.ExpectedToggleStateVersion);
            await _session.Commit();
        }

        public async Task Handle(Commands.DeleteToggle.Command message)
        {
            await _deleteToggleValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteToggle(message.UserId, message.Key, message.ExpectedToggleVersion);
            await _session.Commit();
        }

        public async Task Handle(Commands.DeleteEnvironment.Command message)
        {
            await _deleteEnvironmentValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteEnvironment(message.UserId, message.Key, message.ExpectedEnvironmentVersion);
            await _session.Commit();
        }
    }
}
