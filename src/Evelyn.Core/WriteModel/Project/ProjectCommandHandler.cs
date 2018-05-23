namespace Evelyn.Core.WriteModel.Project
{
    using System.Threading.Tasks;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Domain;
    using FluentValidation;

    public class ProjectCommandHandler :
        ICommandHandler<AddEnvironment>,
        ICommandHandler<AddToggle>,
        ICommandHandler<ChangeToggleState>,
        ICommandHandler<DeleteToggle>,
        ICommandHandler<DeleteEnvironment>
    {
        private readonly ISession _session;
        private readonly AbstractValidator<AddEnvironment> _addEnvironmentValidator;
        private readonly AbstractValidator<AddToggle> _addToggleValidator;
        private readonly AbstractValidator<ChangeToggleState> _changeToggleStateValidator;
        private readonly AbstractValidator<DeleteToggle> _deleteToggleValidator;
        private readonly AbstractValidator<DeleteEnvironment> _deleteEnvironmentValidator;

        public ProjectCommandHandler(ISession session)
        {
            _session = session;
            _addEnvironmentValidator = new AddEnvironmentValidator();
            _addToggleValidator = new AddToggleValidator();
            _changeToggleStateValidator = new ChangeToggleStateValidator();
            _deleteToggleValidator = new DeleteToggleValidator();
            _deleteEnvironmentValidator = new DeleteEnvironmentValidator();
        }

        public async Task Handle(AddEnvironment message)
        {
            await _addEnvironmentValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.AddEnvironment(message.UserId, message.Key, message.ExpectedProjectVersion);
            await _session.Commit();
        }

        public async Task Handle(AddToggle message)
        {
            await _addToggleValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.AddToggle(message.UserId, message.Key, message.Name, message.ExpectedProjectVersion);
            await _session.Commit();
        }

        public async Task Handle(ChangeToggleState message)
        {
            await _changeToggleStateValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.ChangeToggleState(message.UserId, message.EnvironmentKey, message.ToggleKey, message.Value, message.ExpectedToggleStateVersion);
            await _session.Commit();
        }

        public async Task Handle(DeleteToggle message)
        {
            await _deleteToggleValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteToggle(message.UserId, message.Key, message.ExpectedToggleVersion);
            await _session.Commit();
        }

        public async Task Handle(DeleteEnvironment message)
        {
            await _deleteEnvironmentValidator.ValidateAndThrowAsync(message);

            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteEnvironment(message.UserId, message.Key, message.ExpectedEnvironmentVersion);
            await _session.Commit();
        }
    }
}
