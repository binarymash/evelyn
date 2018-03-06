namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Evelyn.Core.ReadModel.Events;
    using Evelyn.Core.ReadModel.Infrastructure;

    public class EnvironmentDetailsHandler
        : ICancellableEventHandler<EnvironmentAdded>
    {
        private readonly IDatabase<Guid, EnvironmentDetailsDto> _db;

        public EnvironmentDetailsHandler(IDatabase<Guid, EnvironmentDetailsDto> db)
        {
            _db = db;
        }

        public Task Handle(EnvironmentAdded message, CancellationToken token)
        {
            _db.AddOrUpdate(message.EnvironmentId, new EnvironmentDetailsDto(message.Id, message.EnvironmentId, message.Name, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
