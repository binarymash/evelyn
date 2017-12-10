namespace Evelyn.Core.ReadModel.EnvironmentDetails
{
    using System;

    public class EnvironmentDetailsDto
    {
        public EnvironmentDetailsDto(Guid applicationId, Guid environmentId, string name, DateTimeOffset created)
        {
            Id = environmentId;
            Name = name;
            Created = created;
            LastModified = created;
        }

        public Guid ApplicationId { get; }

        public Guid Id { get; }

        public string Name { get; private set; }

        public DateTimeOffset Created { get; }

        public DateTimeOffset LastModified { get; private set; }

        public void ShouldBe()
        {
            throw new NotImplementedException();
        }
    }
}
