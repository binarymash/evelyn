namespace Evelyn.Core.WriteModel.Commands
{
    using System;

    public class CreateProject : Command
    {
        public CreateProject(string userId, string accountId, Guid id, string name, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            Name = name;
            AccountId = accountId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string AccountId { get; set; }
    }
}
