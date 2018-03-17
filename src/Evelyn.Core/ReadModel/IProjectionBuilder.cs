namespace Evelyn.Core.ReadModel
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IProjectionBuilder<TRequest, TDto>
    {
        Task<TDto> Invoke(TRequest request, CancellationToken token = default(CancellationToken));
    }
}