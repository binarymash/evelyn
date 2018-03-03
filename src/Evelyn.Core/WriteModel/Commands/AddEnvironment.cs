namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class AddEnvironment : Command
    {
        public AddEnvironment(string userId, Guid applicationId, Guid id, string name, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            Name = name;
            ApplicationId = applicationId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
