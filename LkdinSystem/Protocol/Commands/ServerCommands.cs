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

        public string GetAllUsers()
        {
            var users = fileDatabaseManager.GetAllUsers();

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

        public string GetAllProfiles(string filters)
        {
            var usersProfiles = fileDatabaseManager.GetAllProfiles();

            var splitedFilters = filters.Split(SpecialChars.Separator);

            var userId = splitedFilters[0];
            var description = splitedFilters[1];
            var abilities = splitedFilters[2];

            if (!string.IsNullOrWhiteSpace(userId))
            {
                usersProfiles = usersProfiles.FindAll((e) => e.UserId == userId);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                usersProfiles = usersProfiles.FindAll((e) => e.Description.Contains(description));
            }

            if (!string.IsNullOrWhiteSpace(abilities))
            {
                string[] splitedAbilities = abilities.Split(SpecialChars.ArrayDivider);

                for (int i = 0; i < splitedAbilities.Length; i++)
                {
                    usersProfiles = usersProfiles.FindAll((e) => e.Abilities.Contains(splitedAbilities[i]));
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

        public string SaveNewUser(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Usuario invalido");
            }
            else
            {
                User user = User.ToEntity(message);

                var users = fileDatabaseManager.GetAllUsers();

                User? usu = users.Find((e) => e.Email == user.Email);

                if (usu != null)
                {
                    throw new Exception("Usuario ya registrado");
                }

                fileDatabaseManager.SaveNewUser(user);

                return "Usuario guardado";
            }
        }

        public string SaveNewUserProfile(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Perfil invalido");
            }
            else
            {
                UserProfile userProfile = UserProfile.ToEntity(message);

                var users = fileDatabaseManager.GetAllUsers();

                User? user = users.Find((e) => e.Id == userProfile.UserId);

                if (user == null)
                {
                    throw new Exception("No existe el usuario");
                }

                fileDatabaseManager.SaveNewUserProfile(userProfile);

                return "Perfil guardado correctamente";
            }
        }

        public string SignIn(string message)
        {
            User user = User.ToEntity(message);

            var users = fileDatabaseManager.GetAllUsers();

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

            fileDatabaseManager.EmptyDataBase(usersFileName);

            newUsers.ForEach(user =>
            {
                fileDatabaseManager.SaveNewUser(user);
            });

            return savedUser.ToString();
        }

        public string SignOut(string message)
        {
            User user = User.ToEntity(message);

            var users = fileDatabaseManager.GetAllUsers();

            User? savedUser = users.Find((e) => e.Id == user.Id);

            if (savedUser == null)
            {
                throw new Exception("No existe el usuario");
            }

            var newUsers = users.FindAll((e) => e.Id != user.Id);

            user.CurrentState = "" + User.Status.NotLogged;
            newUsers.Add(user);

            fileDatabaseManager.EmptyDataBase(usersFileName);

            newUsers.ForEach(user =>
            {
                fileDatabaseManager.SaveNewUser(user);
            });

            return "Usuario deslogeado";
        }

        public string UploadUserProfileImage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Usuario invalido");
            }
            else
            {
                UserProfile userProfile = UserProfile.ToEntity(message);

                var userProfiles = fileDatabaseManager.GetAllProfiles();

                UserProfile? savedUserProfile = userProfiles.Find((e) => e.UserId == userProfile.UserId);

                if (savedUserProfile == null)
                {
                    throw new Exception("No existe el perfil");
                }

                var newUsersProfiles = userProfiles.FindAll((e) => e.UserId != userProfile.UserId);

                userProfile.Image = userProfile.Image.Split("\\").Last();

                newUsersProfiles.Add(userProfile);

                fileDatabaseManager.EmptyDataBase(usersProfileFileName);

                newUsersProfiles.ForEach(user =>
                {
                    fileDatabaseManager.SaveNewUserProfile(user);
                });

                return "Perfil actualizado";
            }
        }

        public string DownloadUserProfileImage(string message)
        {
            UserProfile userProfile = UserProfile.ToEntity(message);

            return userProfile.Image;
        }

        public string SaveNewMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new Exception("Mensaje invalido");
            }
            else
            {
                Message msg = Message.ToEntity(message);

                var users = fileDatabaseManager.GetAllUsers();

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

                fileDatabaseManager.SaveNewMessage(msg);

                return "Mensaje guardado correctamente";
            }
        }

        public string GetNotReadedMessages(string receiveStr)
        {
            User user = User.ToEntity(receiveStr);

            List<Message> messages = fileDatabaseManager.GetAllNotReadedUserMessages(user);

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

        public string GetUserMessagesHistory(string receiveStr)
        {
            User user = User.ToEntity(receiveStr);
            List<Message> messages = fileDatabaseManager.GetUserMessagesHistory(user);

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
