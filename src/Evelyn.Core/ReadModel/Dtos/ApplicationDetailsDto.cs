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
            _environments = new Dictionary<Guid, EnvironmentListDto>();
        }

        public Guid Id { get; }

        public string Name { get; }

        public int Version { get; }

        public DateTimeOffset Created { get; }

        public IEnumerable<EnvironmentListDto> Environments
        {
            get
            {
                return _environments.Values;
            }
        }

        public void AddEnvironment(EnvironmentListDto environment)
        {
            _environments.Add(environment.Id, environment);
        }
    }
}
