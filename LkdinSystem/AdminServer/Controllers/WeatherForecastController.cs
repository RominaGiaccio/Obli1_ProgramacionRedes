using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("example")]
    public class WeatherForecastController : ControllerBase
    {

        private Admin.AdminClient client;
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

    }
}