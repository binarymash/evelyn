namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using Newtonsoft.Json;

    public class EnvironmentDetailsDto : DtoRoot
    {
        [JsonConstructor]
        private EnvironmentDetailsDto(Guid projectId, int version, string key, string name, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
            : base(version, created, createdBy, lastModified, lastModifiedBy)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public static EnvironmentDetailsDto Create(Guid projectId, string key, string name, DateTimeOffset created, string createdBy)
        {
            return new EnvironmentDetailsDto(projectId, 0, key, name, created, createdBy, created, createdBy);
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentDetailsDto)}-{projectId}-{environmentKey}";
        }
    }
}
