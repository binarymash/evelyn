namespace Evelyn.Core.WriteModel
{
    using System;
    using CQRSlite.Domain.Exception;

    public abstract class ScopedEntity
    {
        public int LastModifiedVersion { get; protected set; }

        public void AssertVersion(int expectedVersion, Guid aggregateId)
        {
            if (LastModifiedVersion != expectedVersion)
            {
                throw new ConcurrencyException(aggregateId);
            }
        }
    }
}
