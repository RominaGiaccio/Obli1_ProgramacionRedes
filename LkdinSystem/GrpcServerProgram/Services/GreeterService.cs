using Grpc.Core;
using GrpcServerProgram;
using Protocol.Commands;
using System.Xml.Linq;

namespace GrpcServerProgram.Services
{
    public class AdminService : Admin.AdminBase
    {

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public Task<MessageReply> PostUser(UserDTO request, ServerCallContext context, ServerCommands serverCommands)
        {
            //BusinessLogic session = BusinessLogic.GetInstance();
            Console.WriteLine("Se intenta crear usuario con nombre {0}", request.Name);
            //string message = "Se intento crear usuario" + request.Name;//session.SaveNewUserAsync(string message);//(request.Name);;
            var message = serverCommands.SaveNewUserAsync(request.Id +"/#" + request.Name + "/#" + request.Email + "/#" + request.CurrentState);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public Task<MessageReply> PutUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Name);
            var message = "No implementado";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public Task<MessageReply> DeleteUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta eliminar usuario con nombre {0}", request.Name);
            var message = "No implementado";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public Task<MessageReply> PostProfile(ProfileDTO request, ServerCallContext context, ServerCommands serverCommands)
        {
            Console.WriteLine("Se intenta crear usuario con id {0}", request.Id);
            var message = serverCommands.SaveNewUserProfileAsync(request.Id + "/#" + request.Description + "/#" +"" +"/#" + request.Image);
            //var message = serverCommands.SaveNewUserProfileAsync(request.Id + "/#" + request.Description + "/#" + request.Abilities + "/#" + request.Image);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }

        public override Task<MessageReply> PutProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Id);
            var message = "No implementado";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> DeleteProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta eliminar usuario con nombre {0}", request.Id);
            var message = "No implementado";
            return Task.FromResult(new MessageReply { Message = message });
        }

        public Task<MessageReply> PutImageProfile(ProfileDTO request, ServerCallContext context, ServerCommands serverCommands)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Id);
            //var message = serverCommands.UploadUserProfileImageAsync(request.Id + "/#" + request.Description + "/#" + request.Abilities + "/#" + request.Image);
            var message = serverCommands .UploadUserProfileImageAsync(request.Id + "/#" + request.Description + "/#" + "" + "/#" + request.Image);
            return Task.FromResult(new MessageReply { Message = message.Result });
        }
    }
}