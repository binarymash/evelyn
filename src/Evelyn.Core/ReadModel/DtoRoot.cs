namespace Evelyn.Core.ReadModel
{
    using System;

    public abstract class DtoRoot
    {
        protected DtoRoot(DateTimeOffset created, DateTimeOffset lastModified)
        {
            Created = created;
            LastModified = lastModified;
        }

        public DateTimeOffset Created { get; private set; }

        public DateTimeOffset LastModified { get; private set; }
    }
}
