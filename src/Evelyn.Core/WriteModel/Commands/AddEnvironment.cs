namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class AddEnvironment : ICommand
    {
        public AddEnvironment(Guid applicationId, Guid id, string name, int? expectedVersion = null)
        {
            Id = id;
            Name = name;
            ApplicationId = applicationId;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
