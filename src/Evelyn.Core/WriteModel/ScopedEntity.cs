namespace Evelyn.Core.WriteModel
{
    using System;
    using CQRSlite.Domain.Exception;

    public abstract class ScopedEntity
    {
        public int LastModifiedVersion { get; protected set; }

        public void AssertVersion(Guid aggregateId, int? expectedVersion)
        {
            if (expectedVersion.HasValue)
            {
                if (LastModifiedVersion > expectedVersion)
                {
                    throw new ConcurrencyException(aggregateId, expectedVersion.Value, LastModifiedVersion);
                }
            }
        }
    }
}
