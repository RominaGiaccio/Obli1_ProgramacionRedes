using Domain;
using Enums;
using Protocol.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Protocol.Commands
{
    public class ServerCommands
    {
        private static string usersFileName = FileDatabaseManager.usersFileName;
        private static string usersProfileFileName = FileDatabaseManager.usersProfileFileName;

        public static FileDatabaseManager fileDatabaseManager = new FileDatabaseManager();

        public async Task<string> GetAllUsersAsync()
        {
            var users = await fileDatabaseManager.GetAllUsersAsync();

            var message = "";

            for (int i = 0; i < users.Count; i++)
            {
                message += users[i].ToString();

                if (i != users.Count - 1)
                {
                    message += SpecialChars.EndLine;
                }
            }

            return message;
        }

        public async Task<string> GetAllProfilesAsync(string filters)
        {
            var originalUsersProfiles = await fileDatabaseManager.GetAllProfilesAsync();
            var usersProfiles = originalUsersProfiles;

            var splitedFilters = filters.Split(SpecialChars.Separator);

            var userId = splitedFilters[0];
            var description = splitedFilters[1];
            var abilities = splitedFilters[2];

            if (!string.IsNullOrWhiteSpace(userId) || !string.IsNullOrWhiteSpace(description))
            {
                usersProfiles = usersProfiles.FindAll((e) => e.UserId.Contains(userId) || e.Description.Contains(description));
            }

            if (!string.IsNullOrWhiteSpace(abilities))
            {
                string[] splitedAbilities = abilities.Split(SpecialChars.ArrayDivider);

                for (int i = 0; i < splitedAbilities.Length; i++)
                {
                    originalUsersProfiles.FindAll((e) => e.Abilities.Contains(splitedAbilities[i])).ForEach(up =>
                    {
                        if (usersProfiles.Find(u => u.UserId == up.UserId) == null)
                        {
                            usersProfiles.Add(up);
                        }
                    });
                }
            }

            var message = "";

            for (int i = 0; i < usersProfiles.Count; i++)
            {
                message += usersProfiles[i].ToString();

                if (i != usersProfiles.Count - 1)
                {
                    message += SpecialChars.EndLine;
                }
            }

            return message == String.Empty ? EmptyStatesMessages.NoProfilesMessage : message;
        }

        public async Task<string> SaveNewUserAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Usuario invalido");
            }
            else
            {
                User user = User.ToEntity(message);

                var users = await fileDatabaseManager.GetAllUsersAsync();

                User? usu = users.Find((e) => e.Email == user.Email);

                if (usu != null)
                {
                    throw new Exception("Usuario ya registrado");
                }

                await fileDatabaseManager.SaveNewUserAsync(user);

                return "Usuario guardado";
            }
        }

        public async Task<string> SaveNewUserProfileAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Perfil invalido");
            }
            else
            {
                UserProfile userProfile = UserProfile.ToEntity(message);

                var users = await fileDatabaseManager.GetAllUsersAsync();

                User? user = users.Find((e) => e.Id == userProfile.UserId);

                if (user == null)
                {
                    throw new Exception("No existe el usuario");
                }

                await fileDatabaseManager.SaveNewUserProfileAsync(userProfile);

                return "Perfil guardado correctamente";
            }
        }

        public async Task<string> SignInAsync(string message)
        {
            User user = User.ToEntity(message);

            var users = await fileDatabaseManager.GetAllUsersAsync();

            User? savedUser = users.Find((e) => e.Email == user.Email);

            if (savedUser == null)
            {
                throw new Exception("No existe el usuario");
            }

            if (savedUser.CurrentState == "" + User.Status.Logged)
            {
                throw new Exception("Usuario ya logeado");
            }

            var newUsers = users.FindAll((e) => e.Id != savedUser.Id);

            savedUser.CurrentState = "" + User.Status.Logged;
            newUsers.Add(savedUser);

            await fileDatabaseManager.EmptyDataBaseAsync(usersFileName);

            var tasks = new List<Task>();

            newUsers.ForEach(user =>
            {
                tasks.Add(fileDatabaseManager.SaveNewUserAsync(user));
            });

            Task.WaitAll(tasks.ToArray());

            return savedUser.ToString();
        }

        public async Task<string> SignOutAsync(string message)
        {
            User user = User.ToEntity(message);

            var users = await fileDatabaseManager.GetAllUsersAsync();

            User? savedUser = users.Find((e) => e.Id == user.Id);

            if (savedUser == null)
            {
                throw new Exception("No existe el usuario");
            }

            var newUsers = users.FindAll((e) => e.Id != user.Id);

            user.CurrentState = "" + User.Status.NotLogged;
            newUsers.Add(user);

            await fileDatabaseManager.EmptyDataBaseAsync(usersFileName);

            var tasks = new List<Task>();

            newUsers.ForEach(user =>
            {
                tasks.Add(fileDatabaseManager.SaveNewUserAsync(user));
            });

            Task.WaitAll(tasks.ToArray());

            return "Usuario deslogeado";
        }

        public async Task<string> UploadUserProfileImageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Usuario invalido");
            }
            else
            {
                UserProfile userProfile = UserProfile.ToEntity(message);

                var userProfiles = await fileDatabaseManager.GetAllProfilesAsync();

                UserProfile? savedUserProfile = userProfiles.Find((e) => e.UserId == userProfile.UserId);

                if (savedUserProfile == null)
                {
                    throw new Exception("No existe el perfil");
                }

                var newUsersProfiles = userProfiles.FindAll((e) => e.UserId != savedUserProfile.UserId);

                savedUserProfile.Image = userProfile.Image.Split("\\").Last();

                newUsersProfiles.Add(savedUserProfile);

                await fileDatabaseManager.EmptyDataBaseAsync(usersProfileFileName);

                var tasks = new List<Task>();
                newUsersProfiles.ForEach(user =>
                {
                    tasks.Add(fileDatabaseManager.SaveNewUserProfileAsync(user));
                });

                Task.WaitAll(tasks.ToArray());

                return "Perfil actualizado";
            }
        }

        public async Task<string> DownloadUserProfileImageAsync(string message)
        {
            UserProfile userProfile = UserProfile.ToEntity(message);

            var usersProfiles = await fileDatabaseManager.GetAllProfilesAsync();

            var image = usersProfiles?.Find(up => up.UserId == userProfile.UserId)?.Image;

            return !string.IsNullOrWhiteSpace(image) ? image : "No image";
        }

        public async Task<string> SaveNewMessageAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Mensaje invalido");
            }
            else
            {
                Message msg = Message.ToEntity(message);

                var users = await fileDatabaseManager.GetAllUsersAsync();

                User? sender = users.Find((e) => e.Email == msg.SenderEmail);
                User? receiver = users.Find((e) => e.Email == msg.ReceiverEmail);

                if (sender == null)
                {
                    throw new Exception("Usuario emisor no existe");
                }

                if (receiver == null)
                {
                    throw new Exception("Usuario receptor no existe");
                }

                await fileDatabaseManager.SaveNewMessageAsync(msg);

                return "Mensaje guardado correctamente";
            }
        }

        public async Task<string> GetNotReadedMessagesAsync(string receiveStr)
        {
            User user = User.ToEntity(receiveStr);

            List<Message> messages = await fileDatabaseManager.GetAllNotReadedUserMessagesAsync(user);

            var message = "";

            for (int i = 0; i < messages.Count; i++)
            {
                message += messages[i].ToString();

                if (i != messages.Count - 1)
                {
                    message += SpecialChars.EndLine;
                }
            }

            return message == String.Empty ? EmptyStatesMessages.NoMessagesMessage : message;
        }

        public async Task<string> GetUserMessagesHistoryAsync(string receiveStr)
        {
            User user = User.ToEntity(receiveStr);
            List<Message> messages = await fileDatabaseManager.GetUserMessagesHistoryAsync(user);

            var message = "";

            for (int i = 0; i < messages.Count; i++)
            {
                message += messages[i].ToString();

                if (i != messages.Count - 1)
                {
                    message += SpecialChars.EndLine;
                }
            }

            return message == String.Empty ? EmptyStatesMessages.NoMessagesMessage : message;
        }
    }
}
