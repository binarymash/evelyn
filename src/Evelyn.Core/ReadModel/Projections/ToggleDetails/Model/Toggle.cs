namespace Evelyn.Core.ReadModel.Projections.ToggleDetails.Model
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Toggle
    {
        [JsonConstructor]
        public Toggle(Guid projectId, string key, string name, AuditDto audit)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
            Audit = audit;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public AuditDto Audit { get; private set; }

        public static Toggle Create(EventAuditDto eventAudit, Guid projectId, string key, string name)
        {
            return new Toggle(projectId, key, name, AuditDto.Create(eventAudit));
        }
    }
}
