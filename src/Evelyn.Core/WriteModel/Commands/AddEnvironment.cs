namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class AddEnvironment : ICommand
    {
        public AddEnvironment(Guid applicationId, Guid id, string name)
        {
            Id = id;
            Name = name;
            ApplicationId = applicationId;
        }

        public int ExpectedVersion { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
