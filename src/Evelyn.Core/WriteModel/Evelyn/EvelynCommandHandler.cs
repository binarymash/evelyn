namespace Evelyn.Core.WriteModel.Evelyn
{
    using System.Threading.Tasks;
    using Account.Commands;
    using Commands;
    using Commands.CreateSystem;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using Domain;
    using FluentValidation;
    using Command = global::Evelyn.Core.WriteModel.Evelyn.Commands.RegisterAccount.Command;
    using Validator = global::Evelyn.Core.WriteModel.Evelyn.Commands.RegisterAccount.Validator;

    public class EvelynCommandHandler :
        ICommandHandler<Commands.CreateSystem.Command>,
        ICommandHandler<global::Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem.Command>,
        ICommandHandler<Command>
    {
        private readonly ISession _session;
        private readonly AbstractValidator<Commands.CreateSystem.Command> _createSystemValidator;
        private readonly AbstractValidator<global::Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem.Command> _startSystemValidator;
        private readonly AbstractValidator<Command> _registerAccountValidator;

        public EvelynCommandHandler(ISession session)
        {
            _session = session;
            _createSystemValidator = new Commands.CreateSystem.Validator();
            _startSystemValidator = new global::Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem.Validator();
            _registerAccountValidator = new Validator();
        }

        public async Task Handle(Commands.CreateSystem.Command message)
        {
            await _createSystemValidator.ValidateAndThrowAsync(message);

            Evelyn evelyn;
            try
            {
                evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            }
            catch (AggregateNotFoundException)
            {
                evelyn = new Evelyn(Constants.SystemUser, Constants.EvelynSystem);
                await _session.Add(evelyn);

                var defaultAccount = evelyn.RegisterAccount(Constants.SystemUser, Constants.DefaultAccount);
                await _session.Add(defaultAccount);
            }

            await _session.Commit();
        }

        public async Task Handle(global::Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem.Command message)
        {
            await _startSystemValidator.ValidateAndThrowAsync(message);

            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            evelyn.StartSystem(message.UserId);
            await _session.Commit();
        }

        public async Task Handle(Command message)
        {
            await _registerAccountValidator.ValidateAndThrowAsync(message);

            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            var account = evelyn.RegisterAccount(message.UserId, message.AccountId);
            await _session.Add(account);
            await _session.Commit();
        }
    }
}
