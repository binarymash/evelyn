namespace Evelyn.Core.WriteModel.Evelyn.Commands.RegisterAccount
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

            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            var account = evelyn.RegisterAccount(message.UserId, message.AccountId);
            await _session.Add(account);
            await _session.Commit();
        }
    }
}
