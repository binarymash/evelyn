namespace Evelyn.Management.Api.Rest.Write.Toggles.Messages
{
    public class DeleteToggle
    {
        public DeleteToggle(int expectedToggleVersion)
        {
            ExpectedToggleVersion = expectedToggleVersion;
        }

        public int ExpectedToggleVersion { get; set; }
    }
}
