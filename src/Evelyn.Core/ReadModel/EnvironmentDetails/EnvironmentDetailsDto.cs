namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class EnvironmentDetailsDto : DtoRoot
    {
        public EnvironmentDetailsDto(Guid projectId, string key, DateTimeOffset created, DateTimeOffset lastModified)
            : base(created, string.Empty, lastModified, string.Empty)
        {
            Key = key;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }
    }
}
