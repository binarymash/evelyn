namespace Evelyn.Client.Domain
{
    public class ToggleState
    {
        public ToggleState(string key, bool value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        public bool Value { get; private set; }
    }
}
