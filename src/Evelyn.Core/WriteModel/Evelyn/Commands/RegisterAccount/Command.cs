namespace Evelyn.Core.WriteModel.Evelyn.Commands.RegisterAccount
{
    using System;

    public class Command : WriteModel.Command
    {
        public Command(string userId, Guid accountId)
            : base(userId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; set; }
    }
}
