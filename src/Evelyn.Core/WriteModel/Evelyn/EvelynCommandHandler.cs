namespace Evelyn.Core.WriteModel.Evelyn
{
    using System.Threading.Tasks;
    using Account.Commands;
    using Commands;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using CQRSlite.Domain.Exception;
    using Domain;
    using FluentValidation;

    public class EvelynCommandHandler :
        ICommandHandler<CreateSystem>,
        ICommandHandler<StartSystem>,
        ICommandHandler<RegisterAccount>
    {
        private readonly ISession _session;
        private readonly AbstractValidator<CreateSystem> _createSystemValidator;
        private readonly AbstractValidator<StartSystem> _startSystemValidator;
        private readonly AbstractValidator<RegisterAccount> _registerAccountValidator;

        public EvelynCommandHandler(ISession session)
        {
            _session = session;
            _createSystemValidator = new CreateSystemValidator();
            _startSystemValidator = new StartSystemValidator();
            _registerAccountValidator = new RegisterAccountValidator();
        }

        public async Task Handle(CreateSystem message)
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

        public async Task Handle(StartSystem message)
        {
            await _startSystemValidator.ValidateAndThrowAsync(message);

            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            evelyn.StartSystem(message.UserId);
            await _session.Commit();
        }

        public async Task Handle(RegisterAccount message)
        {
            await _registerAccountValidator.ValidateAndThrowAsync(message);

            var evelyn = await _session.Get<Evelyn>(Constants.EvelynSystem);
            var account = evelyn.RegisterAccount(message.UserId, message.AccountId);
            await _session.Add(account);
            await _session.Commit();
        }
    }
}
