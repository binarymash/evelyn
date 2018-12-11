namespace Evelyn.Core.ReadModel.Projections.EnvironmentDetails.Model
{
    using System;
    using Newtonsoft.Json;

    public class Environment
    {
        [JsonConstructor]
        private Environment(Guid projectId, string key, string name, AggregateAudit audit)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
            Audit = audit;
        }

        public Guid ProjectId { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public AggregateAudit Audit { get; private set; }

        public static Environment Create(EventAudit eventAudit, Guid projectId, string key, string name)
        {
            return new Environment(projectId, key, name, AggregateAudit.Create(eventAudit));
        }
    }
}
