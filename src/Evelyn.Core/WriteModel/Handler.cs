namespace Evelyn.Core.WriteModel
{
    using System;
    using System.Threading.Tasks;
    using CQRSlite.Commands;
    using CQRSlite.Domain;
    using FluentValidation;
    using Microsoft.Extensions.Logging;

    public abstract class Handler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ILogger<TCommand> _logger;
        private readonly AbstractValidator<TCommand> _validator;

        protected Handler(ILogger<TCommand> logger, ISession session, AbstractValidator<TCommand> validator)
        {
            _logger = logger;
            Session = session;
            _validator = validator;
        }

        protected ISession Session { get; }

        public async Task Handle(TCommand message)
        {
            try
            {
                _logger.LogInformation("Handling {@messageType} with content {@messageContent}", message.GetType().FullName, message);
                await _validator.ValidateAndThrowAsync(message).ConfigureAwait(false);
                await HandleImpl(message).ConfigureAwait(false);
                await Session.Commit().ConfigureAwait(false);
                _logger.LogInformation("Successfully handled {@messageType}", message.GetType().FullName);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error when handling {@messageType}", message.GetType().FullName);
                throw;
            }
        }

        protected abstract Task HandleImpl(TCommand message);
    }
}
