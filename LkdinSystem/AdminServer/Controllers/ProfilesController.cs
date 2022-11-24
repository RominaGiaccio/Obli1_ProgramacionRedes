using Domain;
using Enums;
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
            string skills = string.Join(SpecialChars.ArrayDivider, profile.Abilities);
            var reply = await client.PostProfileAsync(new ProfileDTO()
            {
                Id = profile.UserId,
                Description = profile.Description,
                Abilities = skills,
                Image = profile.Image
            });
            return ControllerErrorHandler(reply);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProfile([FromRoute] string id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteProfileAsync(new ProfileDTO() { Id = id });
            return ControllerErrorHandler(reply);
        }

        [HttpPut]
        public async Task<ActionResult> PutProfile([FromBody] UserProfile profile)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            string skills = string.Join(SpecialChars.ArrayDivider, profile.Abilities);
            var reply = await client.PutProfileAsync(new ProfileDTO() {
                Id = profile.UserId,
                Description = profile.Description,
                Abilities = skills,
                Image = profile.Image
            });
            return ControllerErrorHandler(reply);
        }

        [HttpDelete("image/{id}")]
        public async Task<ActionResult> DeleteProfileImage([FromRoute] string id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteProfileImageAsync(new ProfileDTO() { Id = id });
            Console.WriteLine(reply.Message);
            return ControllerErrorHandler(reply);
        }

        public ActionResult ControllerErrorHandler(MessageReply reply)
        {
            if (reply.Status.Equals("Error"))
            {
                return BadRequest(reply.Message);
            }
            return Ok(reply.Message);
        }
    }
}
