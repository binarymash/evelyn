namespace Evelyn.Core.WriteModel.Evelyn.Commands
{
    using System;

    public class RegisterAccount : Command
    {
        public RegisterAccount(string userId, Guid accountId)
            : base(userId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; set; }
    }
}
