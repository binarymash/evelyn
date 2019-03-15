namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Events;
    using Newtonsoft.Json;

    public class Project : EvelynAggregateRoot
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
            LastModifiedVersion = -1;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
            _environmentStates = new List<EnvironmentState>();
            IsDeleted = false;
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

        public void AddEnvironment(string userId, string key, string name, int? expectedProjectVersion)
        {
            AssertNotDeleted();
            AssertVersion(expectedProjectVersion);

            if (_environments.Any(e => e.Key == key))
            {
                throw new InvalidOperationException($"There is already an environment with the key {key}");
            }

            if (_environments.Any(e => e.Name == name))
            {
                throw new InvalidOperationException($"There is already an environment with the name {name}");
            }

            var now = DateTimeOffset.UtcNow;

            ApplyChange(new EnvironmentAdded(userId, Id, key, name, now));
            var toggleStates = Toggles.Select(t => new KeyValuePair<string, string>(t.Key, t.DefaultValue));

            ApplyChange(new EnvironmentStateAdded(userId, Id, key, now, toggleStates));
        }

        public void AddToggle(string userId, string key, string name, int? expectedProjectVersion)
        {
            AssertNotDeleted();
            AssertVersion(expectedProjectVersion);

            if (_toggles.Any(t => t.Key == key))
            {
                throw new InvalidOperationException($"There is already a toggle with the key {key}");
            }

            if (_toggles.Any(t => t.Name == name))
            {
                throw new InvalidOperationException($"There is already a toggle with the name {name}");
            }

            var now = DateTimeOffset.UtcNow;

            ApplyChange(new ToggleAdded(userId, Id, key, name, now));

            foreach (var environmentState in _environmentStates)
            {
                ApplyChange(new ToggleStateAdded(userId, Id, environmentState.EnvironmentKey, key, default(bool).ToString(), now));
            }
        }

        public void ChangeToggleState(string userId, string environmentKey, string toggleKey, string value, int? expectedToggleStateVersion)
        {
            AssertNotDeleted();

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

            toggleState.AssertVersion(Id, expectedToggleStateVersion);

            if (!bool.TryParse(value, out var parsedValue))
            {
                throw new InvalidOperationException("Invalid toggle value");
            }

            ApplyChange(new ToggleStateChanged(userId, Id, environmentKey, toggleKey, value, DateTimeOffset.UtcNow));
        }

        public void DeleteToggle(string userId, string key, int? expectedToggleVersion)
        {
            AssertNotDeleted();

            var toggle = _toggles.FirstOrDefault(t => t.Key == key);
            if (toggle == null)
            {
                throw new InvalidOperationException($"There is no toggle with the key {key}");
            }

            toggle.AssertVersion(Id, expectedToggleVersion);

            ApplyChange(new ToggleDeleted(userId, Id, key, DateTimeOffset.UtcNow));

            foreach (var environmentState in _environmentStates)
            {
                ApplyChange(new ToggleStateDeleted(userId, Id, environmentState.EnvironmentKey, key, DateTimeOffset.UtcNow));
            }
        }

        public void DeleteEnvironment(string userId, string key, int? expectedEnvironmentVersion)
        {
            AssertNotDeleted();

            var environment = _environments.FirstOrDefault(t => t.Key == key);
            if (environment == null)
            {
                throw new InvalidOperationException($"There is no environment with the key {key}");
            }

            environment.AssertVersion(Id, expectedEnvironmentVersion);

            ApplyChange(new EnvironmentDeleted(userId, Id, key, DateTimeOffset.UtcNow));
            ApplyChange(new EnvironmentStateDeleted(userId, Id, key, DateTimeOffset.UtcNow, this.Toggles.Select(t => t.Key)));
        }

        public void DeleteProject(string userId, int? expectedProjectVersion)
        {
            AssertVersion(expectedProjectVersion);
            AssertNotDeleted();

            var now = DateTime.UtcNow;

            ApplyChange(new ProjectDeleted(userId, Id, now));

            foreach (var toggle in Toggles)
            {
                ApplyChange(new ToggleDeleted(userId, Id, toggle.Key, DateTimeOffset.UtcNow));
            }

            foreach (var environment in Environments)
            {
                ApplyChange(new EnvironmentDeleted(userId, Id, environment.Key, now));
            }

            foreach (var environmentState in EnvironmentStates)
            {
                ApplyChange(new EnvironmentStateDeleted(userId, Id, environmentState.EnvironmentKey, now, this.Toggles.Select(t => t.Key)));
            }
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
            LastModifiedVersion = 0;
        }

        private void Apply(EnvironmentAdded e)
        {
            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();

            _environments.Add(new Environment(e.Key, e.Name, e.OccurredAt, LastModifiedVersion, e.UserId));
        }

        private void Apply(EnvironmentDeleted e)
        {
            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();

            var environment = _environments.First(t => t.Key == e.Key);
            _environments.Remove(environment);
        }

        private void Apply(ToggleAdded e)
        {
            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();

            var toggle = new Toggle(e.Key, e.Name, e.OccurredAt, LastModifiedVersion, e.UserId);
            _toggles.Add(toggle);
        }

        private void Apply(ToggleDeleted e)
        {
            LastModified = e.OccurredAt;
            LastModifiedBy = e.UserId;
            LastModifiedVersion = CalculateLastModifiedVersion();

            var toggle = _toggles.First(t => t.Key == e.Key);
            _toggles.Remove(toggle);
        }

        private void Apply(EnvironmentStateAdded e)
        {
            var lastModifiedVersion = CalculateLastModifiedVersion();
            var toggleStates = e.ToggleStates.Select(ts => new ToggleState(ts.Key, ts.Value, e.OccurredAt, lastModifiedVersion, e.UserId));
            var environmentState = new EnvironmentState(e.EnvironmentKey, toggleStates, e.OccurredAt, lastModifiedVersion, e.UserId);
            _environmentStates.Add(environmentState);
        }

        private void Apply(EnvironmentStateDeleted e)
        {
            var environmentState = _environmentStates.First(es => es.EnvironmentKey == e.EnvironmentKey);
            _environmentStates.Remove(environmentState);
        }

        private void Apply(ToggleStateAdded e)
        {
            var lastModifiedVersion = CalculateLastModifiedVersion();

            var toggleState = new ToggleState(e.ToggleKey, e.Value, e.OccurredAt, lastModifiedVersion, e.UserId);
            var environmentState = _environmentStates.First(es => es.EnvironmentKey == e.EnvironmentKey);
            environmentState.AddToggleState(toggleState, e.OccurredAt, lastModifiedVersion, e.UserId);
        }

        private void Apply(ToggleStateChanged @event)
        {
            var environmentState = _environmentStates.First(es => es.EnvironmentKey == @event.EnvironmentKey);
            environmentState.SetToggleState(@event.ToggleKey, @event.Value, @event.OccurredAt, CalculateLastModifiedVersion(), @event.UserId);
        }

        private void Apply(ToggleStateDeleted e)
        {
            _environmentStates
                .First(es => es.EnvironmentKey == e.EnvironmentKey)
                .DeleteToggleState(e.ToggleKey, e.OccurredAt, CalculateLastModifiedVersion(), e.UserId);
        }

        private void Apply(ProjectDeleted e)
        {
            LastModifiedVersion = CalculateLastModifiedVersion();

            IsDeleted = true;
        }
    }
}