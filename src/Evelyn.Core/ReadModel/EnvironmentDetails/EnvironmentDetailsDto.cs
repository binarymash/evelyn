namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class EnvironmentDetailsDto
    {
        public EnvironmentDetailsDto(Guid projectId, string key, DateTimeOffset created)
        {
            Key = key;
            Created = created;
            LastModified = created;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
