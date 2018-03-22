namespace Evelyn.Management.Api.Rest.Write.Toggles.Messages
{
    public class AddToggle
    {
        public AddToggle(string key, string name, int expectedVersion)
        {
            Key = key;
            Name = name;
            ExpectedVersion = expectedVersion;
        }

        public string Key { get; set; }

        public string Name { get; set; }

        public int ExpectedVersion { get; set; }
    }
}
