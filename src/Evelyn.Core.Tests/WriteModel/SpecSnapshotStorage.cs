﻿namespace Evelyn.Core.Tests.WriteModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Snapshotting;

    public class SpecSnapShotStorage : ISnapshotStore
    {
        public SpecSnapShotStorage(Snapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public Snapshot Snapshot { get; set; }

        public Task<Snapshot> Get(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Snapshot);
        }

        public Task Save(Snapshot snapshot, CancellationToken cancellationToken = default(CancellationToken))
        {
            Snapshot = snapshot;
            return Task.CompletedTask;
        }
    }
}
