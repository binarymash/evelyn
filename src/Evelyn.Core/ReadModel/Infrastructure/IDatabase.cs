namespace Evelyn.Core.ReadModel.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDatabase<T>
    {
        Task<List<T>> Get();

        Task<T> Get(Guid id);

        Task Add(Guid id, T aggregate);
    }
}
