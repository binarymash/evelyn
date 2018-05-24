namespace Evelyn.Core.WriteModel.Account.Commands.CreateProject
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid id, Guid projectId, string name, int? expectedVersion = null)
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
