namespace Evelyn.Core.ReadModel.Projections.ToggleDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class ToggleDetailsDto : DtoRoot
    {
        [JsonConstructor]
        public ToggleDetailsDto(Guid projectId, string key, string name, ProjectionAuditDto audit, AuditDto toggleAudit)
            : base(audit)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
            ToggleAudit = toggleAudit;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public AuditDto ToggleAudit { get; private set; }

        public static ToggleDetailsDto Create(EventAuditDto eventAudit, Guid projectId, string key, string name)
        {
            return new ToggleDetailsDto(projectId, key, name, ProjectionAuditDto.Create(eventAudit), AuditDto.Create(eventAudit));
        }

        public static string StoreKey(Guid projectId, string toggleKey)
        {
            return $"{nameof(ToggleDetailsDto)}-{projectId}-{toggleKey}";
        }
    }
}
