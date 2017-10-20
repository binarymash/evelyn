namespace Evelyn.Core.ReadModel.Dtos
{
    using System;

    public class ApplicationDetailsDto
    {
        public ApplicationDetailsDto(Guid id, string name, int version, DateTimeOffset created)
        {
            Id = id;
            Name = name;
            Version = version;
            Created = created;
        }

        public Guid Id { get; }

        public string Name { get; }

        public int Version { get; }

        public DateTimeOffset Created { get; }
    }
}
