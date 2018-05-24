namespace Evelyn.Core.WriteModel.Evelyn.Commands.StartSystem
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid id, int? expectedVersion = null)
            : base(userId)
        {
            Id = id;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }
    }
}
