namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;

    public class ToggleDetailsDto
    {
        public ToggleDetailsDto(Guid projectId, string key, string name, DateTimeOffset created)
        {
            Key = key;
            Name = name;
            Created = created;
            LastModified = created;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
