namespace Evelyn.Core.WriteModel.Evelyn.Domain
{
    using System;
    using System.Collections.Generic;
    using Account.Domain;
    using CQRSlite.Domain;
    using Events;

    public class Evelyn : AggregateRoot
    {
        private readonly IList<Guid> _accounts;

        public Evelyn()
        {
            Version = -1;
            _accounts = new List<Guid>();
        }

        public Evelyn(string userId, Guid id)
            : this()
        {
            ApplyChange(new SystemCreated(userId, id));
        }

        public IEnumerable<Guid> Accounts => _accounts;

        public Account RegisterAccount(string userId, Guid accountId)
        {
            if (_accounts.Contains(accountId))
            {
                throw new InvalidOperationException($"There is already an account with the ID {accountId}");
            }

            ApplyChange(new AccountRegistered(userId, this.Id, accountId));
            return new Account(userId, accountId);
        }

        public void StartSystem(string userId)
        {
            ApplyChange(new SystemStarted(userId, this.Id));
        }

        private void Apply(SystemCreated @event)
        {
            this.Id = @event.Id;
        }

        private void Apply(SystemStarted @event)
        {
        }

        private void Apply(AccountRegistered @event)
        {
            _accounts.Add(@event.Id);
        }
    }
}
