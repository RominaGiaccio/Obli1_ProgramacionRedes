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
            Console.WriteLine("logger es: "+logger);
        }

        /* [HttpGet]
         public async Task<ActionResult> Hi()
         {
             using var channel = GrpcChannel.ForAddress(grpcURL);
             client = new Greeter.GreeterClient(channel);
             var reply = await client.SayHelloAsync(new HelloRequest(){ Name = "pape" });
             return Ok(reply.Message);
         }

        [HttpPost("users")]
        public async Task<ActionResult> PostUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteUserAsync(new Id { Id_ = id });
            return Ok(reply.Message);
        }*/

    }
}