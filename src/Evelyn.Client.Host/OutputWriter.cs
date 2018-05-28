namespace Evelyn.Client.Host
{
    public class OutputWriter
    {
        private readonly IEvelynClient _evelyn;

        public OutputWriter(IEvelynClient evelyn)
        {
            _evelyn = evelyn;
        }

        public void WriteToConsole()
        {
            if (_evelyn.GetToggleState("my-first-toggle"))
            {
                System.Console.WriteLine("This code is called when the toggle is ON");
            }
            else
            {
                System.Console.WriteLine("This code is called when the toggle is OFF");
            }
        }
    }
}
