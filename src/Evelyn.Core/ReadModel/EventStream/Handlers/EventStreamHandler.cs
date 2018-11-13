namespace Evelyn.Core.ReadModel.EventStream.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public enum EventStreamHandlerStatus
    {
        Unknown = 0,
        Starting = 1,
        Started = 2,
        Stopping = 3,
        Stopped = 4
    }

    public class EventStreamHandler : BackgroundService
    {
        private readonly ILogger<EventStreamHandler> _logger;
        private readonly EventStream _eventStream;
        private readonly IEventHandler<EventStreamHandler> _eventHandler;

        private CancellationTokenSource _stoppingTokenSource;
        private Task _currentTask;

        public EventStreamHandler(ILogger<EventStreamHandler> logger, IEventStreamFactory eventStreamFactory, IEventHandler<EventStreamHandler> eventHandler)
        {
            _logger = logger;
            _eventStream = eventStreamFactory.GetEventStream<EventStreamHandler>();
            _eventHandler = eventHandler;
        }

        public EventStreamHandlerStatus Status { get; private set; }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Status = EventStreamHandlerStatus.Starting;
            await base.StartAsync(cancellationToken);
            Status = EventStreamHandlerStatus.Started;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Status = EventStreamHandlerStatus.Stopping;

            _stoppingTokenSource.Cancel();

            if (_currentTask != null)
            {
                await Task.WhenAny(_currentTask, Task.Delay(-1, cancellationToken));
            }

            Status = EventStreamHandlerStatus.Stopped;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var eventEnvelope = await _eventStream.DequeueAsync();
                    _currentTask = _eventHandler.HandleEvent(eventEnvelope, stoppingToken);
                    await _currentTask;
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
