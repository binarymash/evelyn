namespace Evelyn.Client.Host
{
    public class InputReader
    {
        private readonly OutputWriter _outputWriter;

        public InputReader(OutputWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public void Run()
        {
            System.Console.WriteLine("Hit enter to continue, or any other key to exit\r\n");

            while (true)
            {
                if (System.Console.ReadKey().Key != System.ConsoleKey.Enter)
                {
                    break;
                }

                _outputWriter.WriteToConsole();
            }
        }
    }
}
