using Domain;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{

    [ApiController]
    [Route("admin")]
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

            [HttpPost("users")]
            public async Task<ActionResult> PostUser(User user)//[FromBody] 
            {
                using var channel = GrpcChannel.ForAddress(grpcURL);
                client = new Admin.AdminClient(channel);
                var reply = await client.PostUserAsync(new UserDTO() { 
                    Id = user.Id , Name = user.Name, CurrentState = user.CurrentState }) ;
                return Ok(reply.Message);
            }

            [HttpDelete("users/{id}")]
            public async Task<ActionResult> DeleteUser(string id)//[FromRoute]
        {
                using var channel = GrpcChannel.ForAddress(grpcURL);
                client = new Admin.AdminClient(channel);
                var reply = await client.DeleteUserAsync(new UserDTO() {Id = id});
            return Ok(reply.Message);
            }

            [HttpPut("users/{id}")]
            public async Task<ActionResult> UpdateUser(string id)//[FromRoute]
        {
                using var channel = GrpcChannel.ForAddress(grpcURL);
                client = new Admin.AdminClient(channel);
                var reply = await client.PutUserAsync(new UserDTO() { Id = id });
            return Ok(reply.Message);
            }

        /*

            [HttpPut]
            public async Task<ActionResult> UpdateUser()
            {
                using var channel = GrpcChannel.ForAddress(grpcURL);
                client = new Greeter.GreeterClient(channel);
                var reply = await client.SayHelloAsync(new HelloRequest() { Name = "loli" });
                return Ok(reply.Message);
            }
      */
    }
}
