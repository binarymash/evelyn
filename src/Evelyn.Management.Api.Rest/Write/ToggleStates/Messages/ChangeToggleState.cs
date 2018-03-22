namespace Evelyn.Management.Api.Rest.Write.ToggleStates.Messages
{
    public class ChangeToggleState
    {
        public ChangeToggleState(int expectedVersion, string state)
        {
            ExpectedVersion = expectedVersion;
            State = state;
        }

        public int ExpectedVersion { get; set; }

        public string State { get; set; }
    }
}
