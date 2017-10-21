namespace Evelyn.Core.ReadModel.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Dtos;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class ApplicationListView
        : ICancellableEventHandler<ApplicationCreated>
    {
        private readonly IDatabase<ApplicationListDto> _applications;

        public ApplicationListView(IDatabase<ApplicationListDto> applications)
        {
            _applications = applications;
        }

        public Task Handle(ApplicationCreated message, CancellationToken token)
        {
            _applications.Add(message.Id, new ApplicationListDto(message.Id, message.Name));
            return Task.CompletedTask;
        }
    }
}
