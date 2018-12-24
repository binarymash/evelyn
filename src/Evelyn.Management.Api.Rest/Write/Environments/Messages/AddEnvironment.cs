namespace Evelyn.Management.Api.Rest.Write.Environments.Messages
{
    using System;

    public class AddEnvironment
    {
        public AddEnvironment(string key, string name, int? expectedProjectVersion)
        {
            Key = key;
            Name = name;
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public string Key { get; set; }

        public string Name { get; set; }

        public int? ExpectedProjectVersion { get; set; }
    }
}
