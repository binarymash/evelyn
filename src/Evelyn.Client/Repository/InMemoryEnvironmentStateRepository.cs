namespace Evelyn.Client.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Domain;

    public class InMemoryEnvironmentStateRepository : IEnvironmentStateRepository, IDisposable
    {
        private ReaderWriterLockSlim _lock;

        private EnvironmentState _environmentState;

        public InMemoryEnvironmentStateRepository()
        {
            _lock = new ReaderWriterLockSlim();
            _environmentState = new EnvironmentState(-1, new List<ToggleState>());
        }

        public void Set(EnvironmentState environmentState)
        {
            if (environmentState?.ToggleStates == null)
            {
                throw new InvalidEnvironmentStateException();
            }

            _lock.EnterWriteLock();
            try
            {
                _environmentState = environmentState;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool Get(string toggleKey)
        {
            _lock.EnterReadLock();
            try
            {
                return _environmentState
                           ?.ToggleStates
                           ?.FirstOrDefault(ts => ts.Key == toggleKey)
                           ?.Value ?? default;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _lock?.Dispose();
            _lock = null;
        }
    }
}
