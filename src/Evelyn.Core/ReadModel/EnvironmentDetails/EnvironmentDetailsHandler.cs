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
        private readonly IDatabase<string, EnvironmentDetailsDto> _db;

        public EnvironmentDetailsHandler(IDatabase<string, EnvironmentDetailsDto> db)
        {
            _db = db;
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            _db.AddOrUpdate(message.Key, new EnvironmentDetailsDto(message.Id, message.Key, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
