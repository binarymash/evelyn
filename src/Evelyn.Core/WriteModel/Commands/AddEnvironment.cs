namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class AddEnvironment : Command
    {
        public AddEnvironment(string userId, Guid projectId, Guid id, string name, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            Name = name;
            ProjectId = projectId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ProjectId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
