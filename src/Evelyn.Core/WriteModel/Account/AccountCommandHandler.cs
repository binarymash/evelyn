namespace Evelyn.Core.WriteModel.Account
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using FluentValidation;

    public class AccountCommandHandler : ICommandHandler<Commands.CreateProject.Command>
    {
        private readonly ISession _session;
        private readonly AbstractValidator<Commands.CreateProject.Command> _createProjectValidator;

        public AccountCommandHandler(ISession session)
        {
            _session = session;
            _createProjectValidator = new Commands.CreateProject.Validator();
        }

        public async Task Handle(Commands.CreateProject.Command message)
        {
            await _createProjectValidator.ValidateAndThrowAsync(message);

            var account = await _session.Get<Domain.Account>(message.Id, message.ExpectedVersion).ConfigureAwait(false);
            var project = account.CreateProject(message.UserId, message.ProjectId, message.Name);
            await _session.Add(project).ConfigureAwait(false);
            await _session.Commit().ConfigureAwait(false);
        }
    }
}
