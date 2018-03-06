namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Events;
    using Infrastructure;

    public class ToggleDetailsHandler
        : ICancellableEventHandler<ToggleAdded>
    {
        private readonly IDatabase<Guid, ToggleDetailsDto> _db;

        public ToggleDetailsHandler(IDatabase<Guid, ToggleDetailsDto> db)
        {
            _db = db;
        }

        public Task Handle(ToggleAdded message, CancellationToken token)
        {
            _db.AddOrUpdate(message.ToggleId, new ToggleDetailsDto(message.Id, message.ToggleId, message.Name, message.Key, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
