namespace Evelyn.Core.ReadModel.ApplicationDetails
{
    using System;
    using System.Collections.Generic;

    public class ApplicationDetailsDto
    {
        private readonly Dictionary<Guid, EnvironmentListDto> _environments;

        public ApplicationDetailsDto(Guid id, string name, int version, DateTimeOffset created)
        {
            Id = id;
            Name = name;
            Version = version;
            Created = created;
            LastModified = created;
            _environments = new Dictionary<Guid, EnvironmentListDto>();
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public int Version { get; private set; }

        public DateTimeOffset Created { get; private set; }

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

        public void AddEnvironment(EnvironmentListDto environment, DateTimeOffset timestamp, int version)
        {
            _environments.Add(environment.Id, environment);
            Version = version;
            LastModified = timestamp;
        }
    }
}
