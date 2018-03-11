namespace Evelyn.Core.WriteModel.Evelyn.Commands
{
    using System;

    public class CreateSystem : Command
    {
        public CreateSystem(string userId, Guid id, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }
    }
}
