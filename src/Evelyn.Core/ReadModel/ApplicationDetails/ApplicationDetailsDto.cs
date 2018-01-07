namespace Evelyn.Core.ReadModel.ApplicationDetails
{
    using System;
    using System.Collections.Generic;

    public class ApplicationDetailsDto
    {
        private readonly Dictionary<Guid, EnvironmentListDto> _environments;
        private readonly Dictionary<Guid, ToggleListDto> _toggles;

        public ApplicationDetailsDto(Guid id, string name, int version, DateTimeOffset created)
        {
            Id = id;
            Name = name;
            Version = version;
            Created = created;
            LastModified = created;
            _environments = new Dictionary<Guid, EnvironmentListDto>();
            _toggles = new Dictionary<Guid, ToggleListDto>();
        }

        public Guid Id { get; }

        public string Name { get; }

        public int Version { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }

        public IEnumerable<EnvironmentListDto> Environments
        {
            get => _environments.Values;

            // ReSharper disable once UnusedMember.Local
            private set
            {
                foreach (var environment in value)
                {
                    _environments.Add(environment.Id, environment);
                }
            }
        }

        public IEnumerable<ToggleListDto> Toggles
        {
            get => _toggles.Values;

            // ReSharper disable once UnusedMember.Local
            private set
            {
                foreach (var toggle in value)
                {
                    _toggles.Add(toggle.Id, toggle);
                }
            }
        }

        public void AddEnvironment(EnvironmentListDto environment, DateTimeOffset timestamp, int version)
        {
            _environments.Add(environment.Id, environment);
            Version = version;
            LastModified = timestamp;
        }

        public void AddToggle(ToggleListDto toggle, DateTimeOffset timestamp, int version)
        {
            _toggles.Add(toggle.Id, toggle);
            Version = version;
            LastModified = timestamp;
        }
    }
}
