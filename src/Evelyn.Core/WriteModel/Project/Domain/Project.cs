﻿namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CQRSlite.Domain;
    using Events;

    public class Project : AggregateRoot
    {
        private List<Environment> _environments;
        private List<Toggle> _toggles;
        private List<EnvironmentState> _environmentStates;

        public Project()
        {
            Version = -1;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
            _environmentStates = new List<EnvironmentState>();
        }

        public Project(string userId, Guid accountId, Guid projectId, string name)
            : this()
        {
            ApplyChange(new ProjectCreated(userId, accountId, projectId, name, DateTimeOffset.UtcNow));
        }

        public IEnumerable<Environment> Environments => _environments.ToList();

        public IEnumerable<Toggle> Toggles => _toggles.ToList();

        public IEnumerable<EnvironmentState> EnvironmentStates => _environmentStates.ToList();

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string Name { get; private set; }

        public void AddEnvironment(string userId, string key)
        {
            var now = DateTimeOffset.UtcNow;

            if (_environments.Any(e => e.Key == key))
            {
                throw new InvalidOperationException($"There is already an environment with the key {key}");
            }

            ApplyChange(new EnvironmentAdded(userId, Id, key, now));
            var toggleStates = Toggles.Select(t => new KeyValuePair<string, string>(t.Key, t.DefaultValue));

            ApplyChange(new EnvironmentStateAdded(userId, Id, key, now, toggleStates));
        }

        public void AddToggle(string userId, string key, string name)
        {
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

        public void ChangeToggleState(string userId, string environmentKey, string toggleKey, string value)
        {
            var environment = _environments.FirstOrDefault(e => e.Key == environmentKey);
            if (environment == null)
            {
                throw new InvalidOperationException($"There is no environment with the key {environmentKey}");
            }

            if (!_toggles.Any(t => t.Key == toggleKey))
            {
                throw new InvalidOperationException($"There is no toggle with the key {toggleKey}");
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
            Name = e.Name;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
            _environmentStates = new List<EnvironmentState>();
            Created = e.OccurredAt;
            LastModified = e.OccurredAt;
        }

        private void Apply(EnvironmentAdded e)
        {
            _environments.Add(new Environment(e.Key, e.OccurredAt));
            LastModified = e.OccurredAt;
        }

        private void Apply(EnvironmentStateAdded e)
        {
            var toggleStates = e.ToggleStates.Select(ts => new ToggleState(ts.Key, ts.Value, e.OccurredAt));
            var environmentState = new EnvironmentState(e.EnvironmentKey, toggleStates, e.OccurredAt);
            _environmentStates.Add(environmentState);
            LastModified = e.OccurredAt;
        }

        private void Apply(ToggleAdded e)
        {
            var toggle = new Toggle(e.Key, e.Name, e.OccurredAt);
            _toggles.Add(toggle);
            LastModified = e.OccurredAt;
        }

        private void Apply(ToggleStateAdded e)
        {
            var toggleState = new ToggleState(e.ToggleKey, e.Value, e.OccurredAt);

            var environmentState = _environmentStates.First(es => es.EnvironmentKey == e.EnvironmentKey);
            environmentState.AddToggleState(toggleState);
            LastModified = e.OccurredAt;
        }

        private void Apply(ToggleStateChanged @event)
        {
            var environmentState = _environmentStates.First(es => es.EnvironmentKey == @event.EnvironmentKey);
            environmentState.SetToggleState(@event.ToggleKey, @event.Value, @event.OccurredAt);
            LastModified = @event.OccurredAt;
        }
    }
}
