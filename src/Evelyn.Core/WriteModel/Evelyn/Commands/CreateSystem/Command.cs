namespace Evelyn.Core.WriteModel.Evelyn.Commands.CreateSystem
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid id)
            : base(userId)
        {
            Id = id;
            ExpectedVersion = -1;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }
    }
}
