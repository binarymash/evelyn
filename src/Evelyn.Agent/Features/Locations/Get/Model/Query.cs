namespace Evelyn.Agent.Features.Locations.Get
{
    using MediatR;
    using Model;

    public class Query : IRequest<BinaryMash.Responses.Response<Locations>>
    {
    }
}
