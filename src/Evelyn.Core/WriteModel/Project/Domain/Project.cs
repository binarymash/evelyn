namespace Evelyn.Core.WriteModel.Project.Domain
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
            ApplyChange(new ProjectCreated(userId, accountId, projectId, name));
        }

        public IEnumerable<Environment> Environments => _environments.ToList();

        public IEnumerable<Toggle> Toggles => _toggles.ToList();

        public IEnumerable<EnvironmentState> EnvironmentStates => _environmentStates.ToList();

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string Name { get; private set; }

        public void AddEnvironment(string userId, string key)
        {
            if (_environments.Any(e => e.Key == key))
            {
                throw new InvalidOperationException($"There is already an environment with the key {key}");
            }

            ApplyChange(new EnvironmentAdded(userId, Id, key));
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

            ApplyChange(new ToggleAdded(userId, Id, key, name));
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

            ApplyChange(new ToggleStateChanged(userId, Id, environmentKey, toggleKey, value));
        }

        private void Apply(ProjectCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
            _environmentStates = new List<EnvironmentState>();
            Created = e.TimeStamp;
            LastModified = e.TimeStamp;
        }

        private void Apply(EnvironmentAdded e)
        {
            _environments.Add(new Environment(e.Key, e.TimeStamp));

            var toggleStates = Toggles.Select(t => new ToggleState(t.Key, t.DefaultValue, e.TimeStamp));

            _environmentStates.Add(new EnvironmentState(e.Key, toggleStates, e.TimeStamp));
            LastModified = e.TimeStamp;
        }

        private void Apply(ToggleAdded e)
        {
            var toggle = new Toggle(e.Key, e.Name, e.TimeStamp);

            _toggles.Add(toggle);

            foreach (var environmentState in _environmentStates)
            {
                environmentState.AddToggleState(new ToggleState(e.Key, toggle.DefaultValue, e.TimeStamp));
            }

            LastModified = e.TimeStamp;
        }

        private void Apply(ToggleStateChanged @event)
        {
            var environmentState = _environmentStates.First(es => es.EnvironmentKey == @event.EnvironmentKey);
            environmentState.SetToggleState(@event.ToggleKey, @event.Value, @event.TimeStamp);
            LastModified = @event.TimeStamp;
        }
    }
}
