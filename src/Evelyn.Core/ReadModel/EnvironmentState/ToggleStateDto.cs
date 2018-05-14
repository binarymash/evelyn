namespace Evelyn.Core.ReadModel.EnvironmentState
{
    public class ToggleStateDto
    {
        public ToggleStateDto(string key, string value, int version)
        {
            Key = key;
            Value = value;
            Version = version;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public int Version { get; private set; }
    }
}
