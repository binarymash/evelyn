namespace Evelyn.Core.ReadModel.Projections.ToggleState
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ProjectEvents = WriteModel.Project.Events;

    public class ProjectionBuilder : ProjectionBuilder<Projection>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.EnvironmentStateDeleted>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateAdded>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateChanged>,
        IBuildProjectionsFrom<ProjectEvents.ToggleStateDeleted>
    {
        public ProjectionBuilder(IProjectionStore<Projection> projectionStore)
            : base(projectionStore)
        {
        }

        public async Task Handle(long streamPosition, ProjectEvents.EnvironmentStateAdded @event, CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();

            foreach (var toggleState in @event.ToggleStates)
            {
                var eventAudit = CreateEventAudit(streamPosition, @event);
                var task = CreateOrUpdateToggleState(eventAudit, @event.Id, @event.EnvironmentKey, toggleState.Key, toggleState.Value);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        public async Task Handle(long streamPosition, ProjectEvents.EnvironmentStateDeleted @event, CancellationToken stoppingToken)
        {
            var tasks = new List<Task>();

            foreach (var toggleKey in @event.ToggleKeys)
            {
                var eventAudit = CreateEventAudit(streamPosition, @event);
                var task = UpdateOrDeleteToggleState(eventAudit, @event.Id, @event.EnvironmentKey, toggleKey);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateAdded @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            await CreateOrUpdateToggleState(eventAudit, @event.Id, @event.EnvironmentKey, @event.ToggleKey, @event.Value);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateChanged @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            var storeKey = Projection.StoreKey(@event.Id, @event.ToggleKey);
            var toggleState = (await Projections.Get(storeKey).ConfigureAwait(false)).ToggleState;

            toggleState.ChangeEnvironmentState(eventAudit, @event.EnvironmentKey, @event.Value);

            var projection = Projection.Create(eventAudit, toggleState);
            await Projections.Update(storeKey, projection).ConfigureAwait(false);
        }

        public async Task Handle(long streamPosition, ProjectEvents.ToggleStateDeleted @event, CancellationToken stoppingToken)
        {
            var eventAudit = CreateEventAudit(streamPosition, @event);
            await UpdateOrDeleteToggleState(eventAudit, @event.Id, @event.EnvironmentKey, @event.ToggleKey);
        }

        private async Task CreateOrUpdateToggleState(EventAudit eventAudit, Guid projectId, string environmentKey, string toggleKey, string toggleValue)
        {
            Projection projection;
            Model.ToggleState toggleState;
            bool toggleWasCreated = false;
            var storeKey = Projection.StoreKey(projectId, toggleKey);

            try
            {
                projection = await Projections.Get(storeKey).ConfigureAwait(false);

                if (projection.Audit.StreamPosition >= eventAudit.StreamPosition)
                {
                    return;
                }

                toggleState = projection.ToggleState;
            }
            catch (ProjectionNotFoundException)
            {
                // TODO: fix this so we don't have to catch the an exception!
                toggleState = Model.ToggleState.Create(eventAudit);
                toggleWasCreated = true;
            }

            toggleState.AddEnvironmentState(eventAudit, environmentKey, toggleValue);
            projection = Projection.Create(eventAudit, toggleState);

            if (toggleWasCreated)
            {
                await Projections.Create(storeKey, projection).ConfigureAwait(false);
            }
            else
            {
                await Projections.Update(storeKey, projection).ConfigureAwait(false);
            }
        }

        private async Task UpdateOrDeleteToggleState(EventAudit eventAudit, Guid projectId, string environmentKey, string toggleKey)
        {
            var storeKey = Projection.StoreKey(projectId, toggleKey);
            var projection = await Projections.Get(storeKey).ConfigureAwait(false);

            if (projection.Audit.StreamPosition >= eventAudit.StreamPosition)
            {
                return;
            }

            var toggleState = projection.ToggleState;
            toggleState.DeleteEnvironmentState(eventAudit, environmentKey);

            projection = Projection.Create(eventAudit, toggleState);
            if (projection.ToggleState.EnvironmentStates.Any())
            {
                await Projections.Update(storeKey, projection).ConfigureAwait(false);
            }
            else
            {
                await Projections.Delete(storeKey).ConfigureAwait(false);
            }
        }
    }
}