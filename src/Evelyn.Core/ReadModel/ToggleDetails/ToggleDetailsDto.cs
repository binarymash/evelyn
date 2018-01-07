namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;

    public class ToggleDetailsDto
    {
        public ToggleDetailsDto(Guid applicationId, Guid toggleId, string name, string key, DateTimeOffset created)
        {
            Id = toggleId;
            Name = name;
            Key = key;
            Created = created;
            LastModified = created;
            ApplicationId = applicationId;
        }

        public Guid ApplicationId { get; private set; }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Key { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
