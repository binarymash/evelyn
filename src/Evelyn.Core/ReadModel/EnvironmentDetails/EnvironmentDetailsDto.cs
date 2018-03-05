namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class EnvironmentDetailsDto
    {
        public EnvironmentDetailsDto(Guid projectId, Guid environmentId, string name, DateTimeOffset created)
        {
            Id = environmentId;
            Name = name;
            Created = created;
            LastModified = created;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
