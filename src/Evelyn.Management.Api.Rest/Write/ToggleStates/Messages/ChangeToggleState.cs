namespace Evelyn.Management.Api.Rest.Write.ToggleStates.Messages
{
    public class ChangeToggleState
    {
        public ChangeToggleState(int expectedToggleStateVersion, string state)
        {
            ExpectedToggleStateVersion = expectedToggleStateVersion;
            State = state;
        }

        public int ExpectedToggleStateVersion { get; set; }

        public string State { get; set; }
    }
}
