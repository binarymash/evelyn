namespace Evelyn.Core.ReadModel.EnvironmentState
{
    public class ToggleStateDto
    {
        public ToggleStateDto(string key, string value, long version)
        {
            Key = key;
            Value = value;
            Version = version;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }

        public long Version { get; private set; }

        public void ChangeState(string value, long version)
        {
            Value = value;
            Version = version;
        }
    }
}
