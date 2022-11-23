using Grpc.Core;
using GrpcServerProgram;
using Protocol;
using Protocol.Commands;
using System.Xml.Linq;

namespace GrpcServerProgram.Services
{
    
    public class AdminService : Admin.AdminBase
    {
        public static ServerCommands serverCommands = new ServerCommands();
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            //BusinessLogic session = BusinessLogic.GetInstance();
            Console.WriteLine("Se intenta crear usuario con nombre {0}", request.Name);
            //string message = "Se intento crear usuario" + request.Name;//session.SaveNewUserAsync(string message);//(request.Name);
            var message = serverCommands.SaveNewUserAsync(request.Id +"/#" + request.Name + "/#" + request.Email + "/#" + request.CurrentState);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public override Task<MessageReply> PutUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Name);
            var message = serverCommands.UploadUserAsync(request.Id + "/#" + request.Name + "/#" + request.Email + "/#" + request.CurrentState);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public override Task<MessageReply> DeleteUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta eliminar usuario con nombre {0}", request.Name);
            var message = serverCommands.DeleteUserAsync(request.Id + "/#" + request.Name + "/#" + request.Email + "/#" + request.CurrentState);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public override Task<MessageReply> PostProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta crear usuario con id {0}", request.Id);
            string[] skills = request.Abilities.Split("/");
            var message = serverCommands.SaveNewUserProfileAsync(request.Id + "/#" + request.Description + "/#" + skills +"/#" + request.Image);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public override Task<MessageReply> PutProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Id);
            string[] skills = request.Abilities.Split("/");
            var message = serverCommands.UploadUserProfileAsync(request.Id + "/#" + request.Description + "/#" + skills + "/#" + request.Image);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public override Task<MessageReply> DeleteProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta eliminar usuario con nombre {0}", request.Id);
            string[] skills = request.Abilities.Split("/");
            var message = serverCommands.DeleteUserProfileAsync(request.Id + "/#" + request.Description + "/#" + skills + "/#" + request.Image);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public Task<MessageReply> PutImageProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Id);
            string[] skills = request.Abilities.Split("/");
            var message = serverCommands.UploadUserProfileImageAsync(request.Id + "/#" + request.Description + "/#" + skills + "/#" + request.Image);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }
    }
}