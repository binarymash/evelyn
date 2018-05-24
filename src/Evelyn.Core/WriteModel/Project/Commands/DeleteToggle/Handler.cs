namespace Evelyn.Core.WriteModel.Project.Commands.DeleteToggle
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using Domain;
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

            var project = await _session.Get<Project>(message.ProjectId);
            project.DeleteToggle(message.UserId, message.Key, message.ExpectedToggleVersion);
            await _session.Commit();
        }
    }
}
