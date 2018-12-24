namespace Evelyn.Core.ReadModel.Projections.AccountProjects
{
    using System;
    using Newtonsoft.Json;

    public class Project
    {
        [JsonConstructor]
        private Project(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public static Project Create(Guid id, string name)
        {
            return new Project(id, name);
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}
