namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;

    public class ToggleDetailsDto
    {
        public ToggleDetailsDto(Guid projectId, string key, string name, DateTimeOffset created, DateTimeOffset lastModified)
        {
            Key = key;
            Name = name;
            Created = created;
            LastModified = lastModified;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
