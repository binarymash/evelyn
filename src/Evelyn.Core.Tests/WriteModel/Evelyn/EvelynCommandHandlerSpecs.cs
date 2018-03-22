namespace Evelyn.Core.Tests.WriteModel.Evelyn
{
    using Core.WriteModel.Evelyn;
    using Core.WriteModel.Evelyn.Domain;
    using CQRSlite.Commands;

    public abstract class EvelynCommandHandlerSpecs<TCommand> : CommandHandlerSpecs<Evelyn, EvelynCommandHandler, TCommand>
        where TCommand : ICommand
    {
        protected override EvelynCommandHandler BuildHandler()
        {
            return new EvelynCommandHandler(Session);
        }
    }
}
