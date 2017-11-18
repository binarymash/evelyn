namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class EnvironmentDetailsHandler
        : ICancellableEventHandler<EnvironmentAdded>
    {
        private readonly IDatabase<EnvironmentDetailsDto> _environments;

        public EnvironmentDetailsHandler(IDatabase<EnvironmentDetailsDto> environments)
        {
            _environments = environments;
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            _environments.Add(message.EnvironmentId, new EnvironmentDetailsDto(message.Id, message.EnvironmentId, message.Name, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
