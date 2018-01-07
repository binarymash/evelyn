namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Events;
    using Infrastructure;

    public class ToggleDetailsHandler
        : ICancellableEventHandler<ToggleAdded>
    {
        private readonly IDatabase<ToggleDetailsDto> _toggles;

        public ToggleDetailsHandler(IDatabase<ToggleDetailsDto> toggles)
        {
            _toggles = toggles;
        }

        public Task Handle(ToggleAdded message, CancellationToken token)
        {
            _toggles.Add(message.ToggleId, new ToggleDetailsDto(message.Id, message.ToggleId, message.Name, message.Key, message.TimeStamp));
            return Task.CompletedTask;
        }
    }
}
