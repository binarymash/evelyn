namespace Evelyn.Core.ReadModel.AccountProjects
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;
    using Infrastructure;

    public class EventStreamHandler : BackgroundService
    {
        private readonly IProjectionBuilder<ProjectionBuilderRequest, AccountProjectsDto> _projectionBuilder;
        private readonly IDatabase<Guid, AccountProjectsDto> _db;
        private readonly Queue<IEvent> _eventsToHandle;

        public EventStreamHandler(IProjectionBuilder<ProjectionBuilderRequest, AccountProjectsDto> projectionBuilder, IDatabase<Guid, AccountProjectsDto> db, IEventStreamFactory eventQueueFactory)
        {
            _projectionBuilder = projectionBuilder;
            _db = db;
            _eventsToHandle = eventQueueFactory.GetEventStream<AccountProjectsDto>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_eventsToHandle.Count > 0)
                {
                    var @event = _eventsToHandle.Dequeue();
                    await UpdateProjection(@event.Id, stoppingToken);
                    System.Diagnostics.Trace.WriteLine("hjkhjk");
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    System.Diagnostics.Trace.WriteLine("hjkhjk");
                }
            }
        }

        private async Task UpdateProjection(Guid id, CancellationToken token)
        {
            var dto = await _projectionBuilder.Invoke(new ProjectionBuilderRequest(id), token).ConfigureAwait(false);
            await _db.AddOrUpdate(id, dto).ConfigureAwait(false);
        }
    }
}
