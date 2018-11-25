namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System;

    public class ToggleDetailsDto : DtoRoot
    {
        public ToggleDetailsDto(Guid projectId, int version, string key, string name, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public static string StoreKey(Guid projectId, string toggleKey)
        {
            return $"{nameof(ToggleDetailsDto)}-{projectId}-{toggleKey}";
        }
    }
}
