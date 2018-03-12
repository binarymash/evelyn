namespace Evelyn.Core.WriteModel.Account.Commands
{
    using System;

    public class CreateProject : Command
    {
        public CreateProject(string userId, Guid id, Guid projectId, string name, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            ProjectId = projectId;
            Name = name;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ProjectId { get; set; }
    }
}
