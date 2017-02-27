namespace Evelyn.Agent.Features.Locations.Get
{
    using MediatR;

    public class Query : IRequest<BinaryMash.Responses.Response<Model.Locations>>
    {
    }
}
