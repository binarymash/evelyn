namespace Evelyn.Core.WriteModel.Domain
{
    using CQRSlite.Domain;
    using Evelyn.Core.ReadModel.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Application : AggregateRoot
    {
        private IList<Environment> _environments;
        private IList<Toggle> _toggles;

        public Application()
        {
        }

        public Application(Guid id, string name)
        {
            Id = id;
            ApplyChange(new ApplicationCreated(id, name));
        }

        public void AddEnvironment(Guid environmentId, string name, string key)
        {
            if (_environments.Any(e => e.Id == environmentId))
            {
                throw new InvalidOperationException($"There is already an environment with the ID {environmentId}");
            }

            if (_environments.Any(e => e.Key == key))
            {
                throw new InvalidOperationException($"There is already an environment with the key {key}");
            }

            if (_environments.Any(e => e.Name == name))
            {
                throw new InvalidOperationException($"There is already an environment with the name {name}");
            }

            ApplyChange(new EnvironmentAdded(Id, environmentId, name, key));
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

        public void FlipToggle(Guid environmentId, Guid toggleId)
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

            ApplyChange(new ToggleFlipped(Id, environmentId, toggleId, !environment.ToggleStates.Any(t => t == toggleId)));
        }

        private void Apply(ApplicationCreated e)
        {
            _environments = new List<Environment>();
            _toggles = new List<Toggle>();
        }

        private void Apply(EnvironmentAdded e)
        {
            _environments.Add(new Environment(e.EnvironmentId, e.Name, e.Key));
        }

        private void Apply(ToggleAdded e)
        {
            _toggles.Add(new Toggle(e.ToggleId, e.Name, e.Key));
        }

        private void Apply(ToggleFlipped @event)
        {
            var environment = _environments.First(e => e.Id == @event.EnvironmentId);
            environment.Toggle(@event.ToggleId);
        }
    }
}
