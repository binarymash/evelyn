namespace Evelyn.Core.ReadModel.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Dtos;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class ApplicationDetailsView
        : ICancellableEventHandler<ApplicationCreated>
    {
        private IDatabase<ApplicationDetailsDto> _applicationDetails;

        public ApplicationDetailsView(IDatabase<ApplicationDetailsDto> applicationDetails)
        {
            _applicationDetails = applicationDetails;
        }

        public Task Handle(ApplicationCreated message, CancellationToken token)
        {
            _applicationDetails.Add(message.Id, new Dtos.ApplicationDetailsDto(message.Id, message.Name, message.Version, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
