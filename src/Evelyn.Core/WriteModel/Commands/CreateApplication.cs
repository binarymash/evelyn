namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class CreateApplication : ICommand
    {
        public CreateApplication(Guid id, string name, int? expectedVersion = null)
        {
            Id = id;
            Name = name;
            ExpectedVersion = expectedVersion;
        }

        public int? ExpectedVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
