namespace Evelyn.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class InMemoryEnvironmentStateRepository : IEnvironmentStateRepository, IDisposable
    {
        private ReaderWriterLockSlim _lock;

        private EnvironmentState _environmentState;

        public InMemoryEnvironmentStateRepository()
        {
            _lock = new ReaderWriterLockSlim();
            _environmentState = new EnvironmentState(-1, new List<ToggleState>());
        }

        public Task Set(EnvironmentState environmentState, CancellationToken cancelletionToken = default(CancellationToken))
        {
            _lock.EnterWriteLock();
            try
            {
                _environmentState = environmentState;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return Task.CompletedTask;
        }

        public Task<bool> Get(string toggleKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            _lock.EnterReadLock();
            try
            {
                var value = _environmentState.ToggleStates.FirstOrDefault(ts => ts.Key == toggleKey).Value;
                return Task.FromResult(bool.Parse(value));
            }
            catch (Exception ex)
            {
                // TODO: logging
            }
            finally
            {
                _lock.ExitReadLock();
            }

            return Task.FromResult(default(bool));
        }

        public void Dispose()
        {
            _lock?.Dispose();
            _lock = null;
        }
    }
}
