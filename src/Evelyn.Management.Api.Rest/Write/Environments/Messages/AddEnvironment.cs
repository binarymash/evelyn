namespace Evelyn.Management.Api.Rest.Write.Environments.Messages
{
    using System;

    public class AddEnvironment
    {
        public AddEnvironment(string key, int expectedProjectVersion)
        {
            Key = key;
            ExpectedProjectVersion = expectedProjectVersion;
        }

        public string Key { get; set; }

        public int ExpectedProjectVersion { get; set; }
    }
}
