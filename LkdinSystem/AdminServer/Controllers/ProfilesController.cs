using Domain;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("admin")]
    public class ProfilesController : ControllerBase
    {
        private Admin.AdminClient client;
        private string grpcURL;

        public ProfilesController(ILogger<ProfilesController> logger)
        {
            AppContext.SetSwitch(
      "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcURL = SettingsManager.ReadSetting(ServerConfig.GrpcURL);

        }

        [HttpPost("profiles")]
        public async Task<ActionResult> PostProfile(UserProfile profile)//[FromBody] 
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostProfileAsync(new ProfileDTO()
            {
                Id = profile.UserId,
                Description = profile.Description,
                //abilities = profile.Abilities,
                Image = profile.Image
                
            });
            return Ok(reply.Message);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(string id)//[FromRoute]
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteProfileAsync(new ProfileDTO() { Id = id });
            return Ok(reply.Message);
        }

        [HttpPut("users/{id}")]
        public async Task<ActionResult> UpdateUser(string id)//[FromRoute]
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PutProfileAsync(new ProfileDTO() { Id = id });
            return Ok(reply.Message);
        }
    }
}
