using Domain;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private Admin.AdminClient client;
        private string grpcURL;

        public UsersController(ILogger<UsersController> logger)
        {
            AppContext.SetSwitch(
        "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcURL = SettingsManager.ReadSetting(ServerConfig.GrpcURL);
                
        }

        [HttpPost]
        public async Task<ActionResult> PostUser([FromBody] User user)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostUserAsync(new UserDTO() { 
                Id = user.Id, Name = user.Name, Email = user.Email, CurrentState = user.CurrentState
            }) ;
            return ControllerErrorHandler(reply);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] string id)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteUserAsync(new UserDTO() {Id = id});
            return ControllerErrorHandler(reply);
         }

          [HttpPut("{id}")]
          public async Task<ActionResult> PutUser([FromBody] User user)
          {
              using var channel = GrpcChannel.ForAddress(grpcURL);
              client = new Admin.AdminClient(channel);
              var reply = await client.PutUserAsync(new UserDTO() { Id = user.Id,
              Name = user.Name, Email = user.Email});
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
