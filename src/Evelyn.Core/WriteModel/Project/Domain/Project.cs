namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CQRSlite.Domain.Exception;
    using Events;
    using Newtonsoft.Json;

    public class Project : EvelynAggregateRoot, IScopedEntity
    {
        [JsonProperty("Environments")]
        private List<Environment> _environments;

        [JsonProperty("Toggles")]
        private List<Toggle> _toggles;

        [JsonProperty("EnvironmentStates")]
        private List<EnvironmentState> _environmentStates;

        public Project()
        {
            Version = -1;
            ScopedVersion = -1;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
            _environmentStates = new List<EnvironmentState>();
        }

        public Project(string userId, Guid accountId, Guid projectId, string name)
            : this()
        {
            ApplyChange(new ProjectCreated(userId, accountId, projectId, name, DateTimeOffset.UtcNow));
        }

        [JsonIgnore]
        public IEnumerable<Environment> Environments => _environments.ToList();

        [JsonIgnore]
        public IEnumerable<Toggle> Toggles => _toggles.ToList();

        [JsonIgnore]
        public IEnumerable<EnvironmentState> EnvironmentStates => _environmentStates.ToList();

        public string Name { get; private set; }

        public int ScopedVersion { get; private set; }

        public void AddEnvironment(string userId, string key, int expectedVersion)
        {
            if (ScopedVersion != expectedVersion)
            {
                throw new ConcurrencyException(Id);
            }

            if (_environments.Any(e => e.Key == key))
            {
                throw new InvalidOperationException($"There is already an environment with the key {key}");
            }

            var now = DateTimeOffset.UtcNow;

            ApplyChange(new EnvironmentAdded(userId, Id, key, now));
            var toggleStates = Toggles.Select(t => new KeyValuePair<string, string>(t.Key, t.DefaultValue));

            ApplyChange(new EnvironmentStateAdded(userId, Id, key, now, toggleStates));
        }

        public void AddToggle(string userId, string key, string name, int expectedVersion)
        {
            if (ScopedVersion != expectedVersion)
            {
                throw new ConcurrencyException(Id);
            }

            if (_toggles.Any(t => t.Key == key))
            {
                throw new InvalidOperationException($"There is already a toggle with the key {key}");
            }

            if (_toggles.Any(e => e.Name == name))
            {
                throw new InvalidOperationException($"There is already a toggle with the name {name}");
            }

            ApplyChange(new ToggleAdded(userId, Id, key, name, DateTimeOffset.UtcNow));

            foreach (var environmentState in _environmentStates)
            {
                ApplyChange(new ToggleStateAdded(userId, Id, environmentState.EnvironmentKey, key, default(bool).ToString(), DateTimeOffset.UtcNow));
            }
        }

        public void ChangeToggleState(string userId, string environmentKey, string toggleKey, string value, int expectedVersion)
        {
            var environmentState = _environmentStates.FirstOrDefault(e => e.EnvironmentKey == environmentKey);
            if (environmentState == null)
            {
                throw new InvalidOperationException($"There is no environment with the key {environmentKey}");
            }

            var toggleState = environmentState.ToggleStates.FirstOrDefault(ts => ts.Key == toggleKey);
            if (toggleState == null)
            {
                throw new InvalidOperationException($"There is no toggle with the key {toggleKey}");
            }

            if (toggleState.ScopedVersion != expectedVersion)
            {
                throw new ConcurrencyException(Guid.Empty);
            }

            if (!bool.TryParse(value, out var parsedValue))
            {
                throw new InvalidOperationException("Invalid toggle value");
            }

            ApplyChange(new ToggleStateChanged(userId, Id, environmentKey, toggleKey, value, DateTimeOffset.UtcNow));
        }

        private void Apply(ProjectCreated e)
        {
            Id = e.Id;
            Created = e.OccurredAt;
            CreatedBy = e.UserId;

            Name = e.Name;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
            _environmentStates = new List<EnvironmentState>();

            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
            ScopedVersion = 0;
        }

        private void Apply(EnvironmentAdded e)
        {
            ScopedVersion++;

            _environments.Add(new Environment(e.Key, e.OccurredAt, e.UserId));

            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
        }

        private void Apply(ToggleAdded e)
        {
            ScopedVersion++;

            var toggle = new Toggle(e.Key, e.Name, e.OccurredAt, e.UserId);
            _toggles.Add(toggle);

            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
        }

        private void Apply(EnvironmentStateAdded e)
        {
            var toggleStates = e.ToggleStates.Select(ts => new ToggleState(ts.Key, ts.Value, e.OccurredAt, e.UserId));
            var environmentState = new EnvironmentState(e.EnvironmentKey, toggleStates, e.OccurredAt, e.UserId);
            _environmentStates.Add(environmentState);
        }

        private void Apply(ToggleStateAdded e)
        {
            var toggleState = new ToggleState(e.ToggleKey, e.Value, e.OccurredAt, e.UserId);

            var environmentState = _environmentStates.First(es => es.EnvironmentKey == e.EnvironmentKey);
            environmentState.AddToggleState(toggleState, e.OccurredAt, e.UserId);
        }

        private void Apply(ToggleStateChanged @event)
        {
            var environmentState = _environmentStates.First(es => es.EnvironmentKey == @event.EnvironmentKey);
            environmentState.SetToggleState(@event.ToggleKey, @event.Value, @event.OccurredAt, @event.UserId);
        }
    }
}
