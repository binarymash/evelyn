namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;
    using WriteModel.Project.Events;

    public class ToggleDetailsHandler
        : ICancellableEventHandler<ToggleAdded>
    {
        private readonly IDatabase<string, ToggleDetailsDto> _db;

        public ToggleDetailsHandler(IDatabase<string, ToggleDetailsDto> db)
        {
            _db = db;
        }

        public Task Handle(ToggleAdded message, CancellationToken token)
        {
            _db.AddOrUpdate(message.Key, new ToggleDetailsDto(message.Id, message.Key, message.Name, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
