namespace Evelyn.Core.ReadModel.Dtos
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

        public Guid Id { get; }

        public string Name { get; private set; }

        public int Version { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }

        public IEnumerable<EnvironmentListDto> Environments
        {
            get
            {
                return _environments.Values;
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
