using Grpc.Core;
using Protocol.Commands;

namespace GrpcServerProgram.Services
{
    public class AdminService : Admin.AdminBase
    {
        public static ServerCommands serverCommands = new ServerCommands();

        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta crear usuario con nombre {0}", request.Name);
            try
            {
                var newID = Guid.NewGuid();
                var message = serverCommands.SaveNewUserAsync(newID + "/#" + request.Name + "/#" + request.Email + "/#" + "NotLogged");
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }

        public override Task<MessageReply> PutUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Name);
            try
            {
                var message = serverCommands.UploadUserAsync(request.Id + "/#" + request.Name + "/#" + request.Email + "/#" + request.CurrentState);
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }

        public override Task<MessageReply> DeleteUser(UserDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta eliminar usuario con nombre {0}", request.Name);
            try
            {
                var message = serverCommands.DeleteUserAsync(request.Id + "/#" + request.Name + "/#" + request.Email + "/#" + request.CurrentState);
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }

        public override Task<MessageReply> PostProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta crear usuario con id {0}", request.Id);
            try
            {
                var message = serverCommands.SaveNewUserProfileAsync(request.Id + "/#" + request.Description + "/#" + request.Abilities + "/#" + request.Image);
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }

        public override Task<MessageReply> PutProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Id);
            try
            {
                var message = serverCommands.UploadUserProfileAsync(request.Id + "/#" + request.Description + "/#" + request.Abilities + "/#" + request.Image);
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }

        public override Task<MessageReply> DeleteProfile(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta eliminar usuario con nombre {0}", request.Id);
            try
            {
                var message = serverCommands.DeleteUserProfileAsync(request.Id + "/#" + request.Description + "/#" + request.Abilities + "/#" + request.Image);
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }

        public override Task<MessageReply> DeleteProfileImage(ProfileDTO request, ServerCallContext context)
        {
            Console.WriteLine("Se intenta actualizar usuario con nombre {0}", request.Id);
            try
            {
                var message = serverCommands.DeleteProfileImageAsync(request.Id + "/#" + request.Description + "/#" + request.Abilities + "/#" + request.Image);
                return Task.FromResult(new MessageReply { Message = message.Result, Status = "Ok" });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new MessageReply { Message = ex.Message, Status = "Error" });
            }
        }
    }
}