namespace Evelyn.Core.WriteModel.Project.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CQRSlite.Domain;
    using Evelyn.Core.ReadModel.Events;

    public class Project : AggregateRoot
    {
        private IList<Environment> _environments;
        private IList<Toggle> _toggles;

        public Project()
        {
            Version = -1;
        }

        public Project(string userId, string accountId, Guid projectId, string name)
            : this()
        {
            ApplyChange(new ProjectCreated(userId, accountId, projectId, name));
        }

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
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
        }

        private void Apply(EnvironmentAdded e)
        {
            _environments.Add(new Environment(e.Key));
        }

        private void Apply(ToggleAdded e)
        {
            _toggles.Add(new Toggle(e.Key, e.Name));
        }

        private void Apply(ToggleStateChanged @event)
        {
        }
    }
}
