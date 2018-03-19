namespace Evelyn.Core.ReadModel
{
    using System;

    public abstract class DtoRoot
    {
        protected DtoRoot(DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
        {
            Created = created;
            CreatedBy = createdBy;
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
        }

        public DateTimeOffset Created { get; private set; }

        public string CreatedBy { get; private set; }

        public DateTimeOffset LastModified { get; private set; }

        public string LastModifiedBy { get; private set; }
    }
}
