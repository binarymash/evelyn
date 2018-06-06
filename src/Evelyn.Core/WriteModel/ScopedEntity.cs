namespace Evelyn.Core.WriteModel
{
    using System;
    using CQRSlite.Domain.Exception;

    public abstract class ScopedEntity
    {
        public int ScopedVersion { get; protected set; }

        public void AssertVersion(int expectedVersion, Guid aggregateId)
        {
            if (ScopedVersion != expectedVersion)
            {
                throw new ConcurrencyException(aggregateId);
            }
        }
    }
}
