namespace Evelyn.Core.ReadModel.ToggleDetails
{
    using System;

    public class ToggleDetailsDto : DtoRoot
    {
        public ToggleDetailsDto(Guid projectId, string key, string name, DateTimeOffset created, DateTimeOffset lastModified)
            : base(created, string.Empty, lastModified, string.Empty)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }
    }
}
