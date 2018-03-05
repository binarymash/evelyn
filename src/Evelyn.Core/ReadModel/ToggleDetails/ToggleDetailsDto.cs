namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;

    public class ToggleDetailsDto
    {
        public ToggleDetailsDto(Guid projectId, Guid toggleId, string name, string key, DateTimeOffset created)
        {
            Id = toggleId;
            Name = name;
            Key = key;
            Created = created;
            LastModified = created;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
