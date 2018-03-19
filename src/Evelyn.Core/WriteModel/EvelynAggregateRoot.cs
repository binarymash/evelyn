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
    }
}
