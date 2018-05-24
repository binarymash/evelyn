namespace Evelyn.Core.WriteModel
{
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using FluentValidation;

    public abstract class Handler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly AbstractValidator<TCommand> _validator;

        protected Handler(ISession session, AbstractValidator<TCommand> validator)
        {
            Session = session;
            _validator = validator;
        }

        protected ISession Session { get; }

        public async Task Handle(TCommand message)
        {
            await _validator.ValidateAndThrowAsync(message).ConfigureAwait(false);
            await HandleImpl(message).ConfigureAwait(false);
            await Session.Commit().ConfigureAwait(false);
        }

        protected abstract Task HandleImpl(TCommand message);
    }
}
