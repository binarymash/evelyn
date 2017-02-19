using MediatR;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Evelyn.Host.Cli
{
    public class Application
    {
        IMediator mediator;

        public Application(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Run()
        {
            var response = await mediator.Send(new Agent.Features.Locations.Get.Model.Query());
            System.Console.WriteLine(JsonConvert.SerializeObject(response));
        }
    }
}
