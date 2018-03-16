namespace Evelyn.Core.ReadModel.EnvironmentState
{
    public class ToggleStateDto
    {
        public ToggleStateDto()
        {
            Version = -1;
        }

        public ToggleStateDto(int version, string key, string value)
        {
            Version = version;
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public int Version { get; private set; }
    }
}
