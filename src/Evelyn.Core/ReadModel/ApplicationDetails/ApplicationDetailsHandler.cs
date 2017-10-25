namespace Evelyn.Core.ReadModel.ApplicationDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class ApplicationDetailsHandler :
        ICancellableEventHandler<ApplicationCreated>,
        ICancellableEventHandler<EnvironmentAdded>
    {
        private IDatabase<ApplicationDetailsDto> _applicationDetails;

        public ApplicationDetailsHandler(IDatabase<ApplicationDetailsDto> applicationDetails)
        {
            _applicationDetails = applicationDetails;
        }

        public Task Handle(ApplicationCreated message, CancellationToken token)
        {
            _applicationDetails.Add(message.Id, new ApplicationDetailsDto(message.Id, message.Name, message.Version, message.TimeStamp));
            return Task.CompletedTask;
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            var applicationDetails = _applicationDetails.Get(message.Id);
            applicationDetails.AddEnvironment(new EnvironmentListDto(message.EnvironmentId, message.Name), message.TimeStamp, message.Version);
            return Task.CompletedTask;
        }
    }
}
