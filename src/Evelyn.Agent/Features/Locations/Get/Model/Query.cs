namespace Evelyn.Agent.Features.Locations.Get.Model
{
    using MediatR;

    public class Query : IRequest<BinaryMash.Responses.Response<Locations>>
    {
    }
}
