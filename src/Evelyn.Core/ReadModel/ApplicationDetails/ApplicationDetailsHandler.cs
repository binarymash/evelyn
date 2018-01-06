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

        public async Task Handle(ApplicationCreated message, CancellationToken token)
        {
            await _applicationDetails.Add(message.Id, new ApplicationDetailsDto(message.Id, message.Name, message.Version, message.TimeStamp));
        }

        public async Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            var applicationDetails = await _applicationDetails.Get(message.Id);
            applicationDetails.AddEnvironment(new EnvironmentListDto(message.EnvironmentId, message.Name), message.TimeStamp, message.Version);
        }
    }
}
