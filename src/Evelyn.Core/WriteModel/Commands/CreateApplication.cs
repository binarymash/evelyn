namespace Evelyn.Core.WriteModel.Commands
{
    using System;
    using CQRSlite.Commands;

    public class CreateApplication : ICommand
    {
        public CreateApplication(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public int ExpectedVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
