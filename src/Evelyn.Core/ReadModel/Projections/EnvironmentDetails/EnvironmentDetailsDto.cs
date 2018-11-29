namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class EnvironmentDetailsDto : DtoRoot
    {
        [JsonConstructor]
        private EnvironmentDetailsDto(Guid projectId, string key, string name, AuditDto audit)
            : base(audit)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public static EnvironmentDetailsDto Create(Guid projectId, string key, string name, DateTimeOffset occurredAt, string initiatedBy, long newVersion)
        {
            return new EnvironmentDetailsDto(projectId, key, name, AuditDto.Create(occurredAt, initiatedBy, newVersion));
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentDetailsDto)}-{projectId}-{environmentKey}";
        }
    }
}
