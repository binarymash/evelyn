namespace Evelyn.Core.WriteModel.Project.Commands
{
    using System;

    public class AddToggle : Command
    {
        public AddToggle(string userId, Guid projectId, string key, string name, int expectedProjectVersion)
            : base(userId)
        {
            Key = key;
            Name = name;
            ProjectId = projectId;
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public int ExpectedProjectVersion { get; set; }

        public Guid ProjectId { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}
