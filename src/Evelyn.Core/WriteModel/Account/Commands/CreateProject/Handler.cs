namespace Evelyn.Core.WriteModel.Account.Commands.CreateProject
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using FluentValidation;

    public class Handler : ICommandHandler<Command>
    {
        private readonly ISession _session;
        private readonly AbstractValidator<Command> _validator;

        public Handler(ISession session)
        {
            _session = session;
            _validator = new Validator();
        }

        public async Task Handle(Command message)
        {
            await _validator.ValidateAndThrowAsync(message);

            var account = await _session.Get<Domain.Account>(message.Id, message.ExpectedVersion).ConfigureAwait(false);
            var project = account.CreateProject(message.UserId, message.ProjectId, message.Name);
            await _session.Add(project).ConfigureAwait(false);
            await _session.Commit().ConfigureAwait(false);
        }
    }
}
