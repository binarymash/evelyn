namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public interface IProjectionBuilderRegistrar
    {
        void Register(Type eventStreamHandlerType, IEnumerable<Type> projectionBuilderTypes);

        Dictionary<Type, List<Func<IEvent, CancellationToken, Task>>> Get(Type eventStreamHandlerType);
    }
}