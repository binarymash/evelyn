namespace Evelyn.Core.ReadModel
{
    using System;

    public abstract class DtoRoot
    {
        protected DtoRoot(long version, DateTimeOffset created, string createdBy, DateTimeOffset lastModified, string lastModifiedBy)
        {
            Version = version;
            Created = created;
            CreatedBy = createdBy;
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
        }

        public DateTimeOffset Created { get; protected set; }

        public string CreatedBy { get; protected set; }

        public DateTimeOffset LastModified { get; protected set; }

        public string LastModifiedBy { get; protected set; }

        public long Version { get; protected set; }

        protected void UpdateModificationAudit(DateTimeOffset lastModified, string lastModifiedBy, long lastModifiedVersion)
        {
            LastModified = lastModified;
            LastModifiedBy = lastModifiedBy;
            Version = lastModifiedVersion;
        }
    }
}
