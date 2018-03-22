namespace Evelyn.Core.ReadModel.EnvironmentState
{
    public class ToggleStateDto
    {
        public ToggleStateDto(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }
    }
}
