namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class EnvironmentDetailsDto
    {
        public EnvironmentDetailsDto(Guid projectId, string key, DateTimeOffset created, DateTimeOffset modified)
        {
            Key = key;
            Created = created;
            LastModified = modified;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
