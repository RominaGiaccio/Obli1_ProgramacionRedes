using Domain;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("profiles")]
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

        [HttpPost]
        public async Task<ActionResult> PostProfile([FromBody] UserProfile profile)
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProfile([FromRoute] string id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteProfileAsync(new ProfileDTO() { Id = id });
            return Ok(reply.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UploadProfile([FromRoute] string id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PutProfileAsync(new ProfileDTO() { Id = id });
            return Ok(reply.Message);
        }

        [HttpDelete("image/{id}")]
        public async Task<ActionResult> DeleteProfileImage([FromRoute] string id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteProfileImageAsync(new ProfileDTO() { Id = id });
            return Ok(reply.Message);
        }
    }
}
