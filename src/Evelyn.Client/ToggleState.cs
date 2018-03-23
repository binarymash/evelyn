namespace Evelyn.Client
{
    public class ToggleState
    {
        public ToggleState(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public string Value { get; private set; }
    }
}
