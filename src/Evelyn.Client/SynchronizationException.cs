namespace Evelyn.Client
{
    using System;

    public class SynchronizationException : Exception
    {
        public SynchronizationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
