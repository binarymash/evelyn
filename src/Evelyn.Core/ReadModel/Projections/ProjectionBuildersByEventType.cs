namespace Evelyn.Core.ReadModel.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CQRSlite.Events;

    public class ProjectionBuildersByEventType : Dictionary<Type, List<Func<IEvent, CancellationToken, Task>>>
    {
        public static ProjectionBuildersByEventType Null => new ProjectionBuildersByEventType();
    }
}
