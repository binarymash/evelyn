namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails.Model
{
    using System;
    using Evelyn.Core.ReadModel.Projections.Shared;
    using Newtonsoft.Json;

    public class Environment
    {
        [JsonConstructor]
        private Environment(Guid projectId, string key, string name, AuditDto audit)
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

        public static Environment Create(EventAuditDto eventAudit, Guid projectId, string key, string name)
        {
            return new Environment(projectId, key, name, AuditDto.Create(eventAudit));
        }
    }
}
