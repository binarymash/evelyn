namespace Evelyn.Management.Api.Rest.Write.Environments.Messages
{
    using System;

    public class AddEnvironment
    {
        public AddEnvironment(string key, int expectedVersion)
        {
            Key = key;
            ExpectedVersion = expectedVersion;
        }

        public string Key { get; set; }

        public int ExpectedVersion { get; set; }
    }
}
