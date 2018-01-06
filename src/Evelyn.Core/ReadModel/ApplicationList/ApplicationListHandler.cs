namespace Evelyn.Core.ReadModel.ApplicationList
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class ApplicationListHandler
        : ICancellableEventHandler<ApplicationCreated>
    {
        private readonly IDatabase<ApplicationListDto> _applications;

        public ApplicationListHandler(IDatabase<ApplicationListDto> applications)
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
