namespace Evelyn.Core.WriteModel
{
    using System;

    public class ConcurrencyException : CQRSlite.Domain.Exception.ConcurrencyException
    {
        public ConcurrencyException(Guid id, int expectedVersion, int actualVersion)
            : base(id)
        {
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }

        public int ExpectedVersion { get; }

        public int ActualVersion { get; }
    }
}
