namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public interface IDatabase<T>
    {
        List<T> Get();

        T Get(Guid id);

        void Add(Guid id, T aggregate);
    }
}
