namespace Evelyn.Management.Api.Rest.Write.Environments.Messages
{
    using System;

    public class AddEnvironment
    {
        public AddEnvironment(Guid id, string name, string key, int expectedVersion)
        {
            Id = id;
            Name = name;
            Key = key;
            ExpectedVersion = expectedVersion;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public int ExpectedVersion { get; set; }
    }
}
