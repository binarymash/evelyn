namespace Evelyn.Core.WriteModel
{
    using System;
    using CQRSlite.Domain;

    public class EvelynAggregateRoot : AggregateRoot
    {
        public DateTimeOffset Created { get; protected set; }

        public string CreatedBy { get; protected set; }

        public DateTimeOffset LastModified { get; protected set; }

        public string LastModifiedBy { get; protected set; }

        public int LastModifiedVersion { get; protected set; }

        public bool IsDeleted { get; protected set; }

        protected int CalculateLastModifiedVersion()
        {
            // Version is only updated *after* an event has been applied
            return Version + 1;
        }

        protected void AssertVersion(int? expectedVersion)
        {
            if (expectedVersion.HasValue)
            {
                if (LastModifiedVersion > expectedVersion.Value)
                {
                    throw new ConcurrencyException(Id, expectedVersion.Value, LastModifiedVersion);
                }
            }
        }

        protected void AssertNotDeleted()
        {
            if (IsDeleted)
            {
                throw new InvalidOperationException($"The project with id {Id} has already been deleted");
            }
        }
    }
}
