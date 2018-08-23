namespace Evelyn.Client.Host
{
    public class Application
    {
        private readonly ClassWithToggle _outputWriter;

        public Application(ClassWithToggle outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public void Run()
        {
            while (true)
            {
                _outputWriter.DoSomething();
            }
        }
    }
}
