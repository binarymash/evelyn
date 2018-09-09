namespace Evelyn.Server.Host
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    public class StatusController : ControllerBase
    {
        [Route("/status/health")]
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok();
        }
    }
}
