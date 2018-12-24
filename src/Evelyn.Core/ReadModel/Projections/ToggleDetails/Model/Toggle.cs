namespace Evelyn.Core.ReadModel.Projections.ToggleDetails.Model
{
    using System;
    using Newtonsoft.Json;

    public class Toggle
    {
        [JsonConstructor]
        public Toggle(Guid projectId, string key, string name, AggregateAudit audit)
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

        public static Toggle Create(EventAudit eventAudit, Guid projectId, string key, string name)
        {
            return new Toggle(projectId, key, name, AggregateAudit.Create(eventAudit));
        }
    }
}
