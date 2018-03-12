namespace Evelyn.Core.WriteModel.Evelyn.Commands
{
    using System;

    public class CreateSystem : Command
    {
        public CreateSystem(string userId, Guid id)
            : base(userId)
        {
            Id = id;
            ExpectedVersion = -1;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }
    }
}
