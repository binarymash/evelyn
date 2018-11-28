namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class ToggleDetailsDto : DtoRoot
    {
        [JsonConstructor]
        public ToggleDetailsDto(Guid projectId, string key, string name, AuditDto audit)
            : base(audit)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public static ToggleDetailsDto Create(Guid projectId, string key, string name, DateTimeOffset created, string createdBy, int version)
        {
            return new ToggleDetailsDto(projectId, key, name, AuditDto.Create(created, createdBy, version));
        }

        public static string StoreKey(Guid projectId, string toggleKey)
        {
            return $"{nameof(ToggleDetailsDto)}-{projectId}-{toggleKey}";
        }
    }
}
