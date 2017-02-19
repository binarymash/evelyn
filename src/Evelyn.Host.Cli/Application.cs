namespace Evelyn.Host.Cli
{
    using System.Threading.Tasks;
    using MediatR;
    using Newtonsoft.Json;

    public class Application
    {
        private IMediator mediator;

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
