using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("example")]
    public class WeatherForecastController : ControllerBase
    {

        private Greeter.GreeterClient client;
        private string grpcURL;

    
        private readonly ILogger<WeatherForecastController> _logger;
        

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            AppContext.SetSwitch(
      "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcURL = SettingsManager.ReadSetting("GrpcURL");
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Hi()
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest(){ Name = "pape" });
            return Ok(reply.Message);
        }
    }
}