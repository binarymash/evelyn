namespace Evelyn.Core.WriteModel.Account.Events
{
    using System;

    public class AccountRegistered : Event
    {
        public AccountRegistered(string userId, Guid accountId)
            : base(userId, accountId)
        {
            Version = -1;
        }
    }
}
