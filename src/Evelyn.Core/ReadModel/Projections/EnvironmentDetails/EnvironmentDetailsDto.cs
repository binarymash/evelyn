namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class EnvironmentDetailsDto : DtoRoot
    {
        [JsonConstructor]
        private EnvironmentDetailsDto(Guid projectId, string key, string name, ProjectionAuditDto audit, AuditDto environmentAudit)
            : base(audit)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
            EnvironmentAudit = environmentAudit;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public AuditDto EnvironmentAudit { get; private set; }

        public static EnvironmentDetailsDto Create(EventAuditDto eventAudit, Guid projectId, string key, string name)
        {
            return new EnvironmentDetailsDto(projectId, key, name, ProjectionAuditDto.Create(eventAudit), AuditDto.Create(eventAudit));
        }

        public static string StoreKey(Guid projectId, string environmentKey)
        {
            return $"{nameof(EnvironmentDetailsDto)}-{projectId}-{environmentKey}";
        }
    }
}
