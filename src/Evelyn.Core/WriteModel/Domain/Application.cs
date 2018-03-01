namespace Evelyn.Core.WriteModel.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CQRSlite.Domain;
    using Evelyn.Core.ReadModel.Events;

    public class Application : AggregateRoot
    {
        private IList<Environment> _environments;
        private IList<Toggle> _toggles;

        public Application()
        {
            Version = -1;
        }

        public Application(Guid id, string name)
            : this()
        {
            ApplyChange(new ApplicationCreated(id, name));
        }

        public void AddEnvironment(Guid environmentId, string name)
        {
            if (_environments.Any(e => e.Id == environmentId))
            {
                throw new InvalidOperationException($"There is already an environment with the ID {environmentId}");
            }

            if (_environments.Any(e => e.Name == name))
            {
                throw new InvalidOperationException($"There is already an environment with the name {name}");
            }

            ApplyChange(new EnvironmentAdded(Id, environmentId, name));
        }

        public void AddToggle(Guid toggleId, string name, string key)
        {
            if (_toggles.Any(t => t.Id == toggleId))
            {
                throw new InvalidOperationException($"There is already a toggle with the ID {toggleId}");
            }

            if (_toggles.Any(t => t.Key == key))
            {
                throw new InvalidOperationException($"There is already a toggle with the key {key}");
            }

            if (_toggles.Any(e => e.Name == name))
            {
                throw new InvalidOperationException($"There is already a toggle with the name {name}");
            }

            ApplyChange(new ToggleAdded(Id, toggleId, name, key));
        }

        public void ChangeToggleState(Guid environmentId, Guid toggleId, string value)
        {
            var environment = _environments.FirstOrDefault(e => e.Id == environmentId);
            if (environment == null)
            {
                throw new InvalidOperationException($"There is no environment with the ID {environmentId}");
            }

            if (!_toggles.Any(t => t.Id == toggleId))
            {
                throw new InvalidOperationException($"There is no toggle with the ID {toggleId}");
            }

            if (!bool.TryParse(value, out var parsedValue))
            {
                throw new InvalidOperationException("Invalid toggle value");
            }

            ApplyChange(new ToggleStateChanged(Id, environmentId, toggleId, value));
        }

        private void Apply(ApplicationCreated e)
        {
            Id = e.Id;
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
        }

        private void Apply(EnvironmentAdded e)
        {
            _environments.Add(new Environment(e.EnvironmentId, e.Name));
        }

        private void Apply(ToggleAdded e)
        {
            _toggles.Add(new Toggle(e.ToggleId, e.Name, e.Key));
        }

        private void Apply(ToggleStateChanged @event)
        {
            var environment = _environments.First(e => e.Id == @event.EnvironmentId);
            environment.Toggle(@event.ToggleId);
        }
    }
}
