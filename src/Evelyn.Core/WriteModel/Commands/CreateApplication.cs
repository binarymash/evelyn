using CQRSlite.Commands;
using System;

namespace Evelyn.Core.WriteModel.Commands
{
    public class CreateApplication : ICommand
    {
        public int ExpectedVersion { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public CreateApplication(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
