using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Enums;
using Microsoft.VisualBasic;
using Protocol.Commands;
using Utils;

namespace Protocol
{
    public class FileDatabaseManager
    {
        public static string usersFileName = SettingsManager.ReadSettings("UsersFileName");
        public static string usersProfileFileName = SettingsManager.ReadSettings("UsersProfilesFileName");
        public static string messagesFileName = SettingsManager.ReadSettings("MessagesFileName");

        public async Task SaveNewUserAsync(User user)
        {
            string fileName = usersFileName;
            string message = user.ToString() + SpecialChars.EndLine;
            byte[] data = ConversionHandler.ConvertStringToBytes(message);

            await FileStreamHandler.WriteAsync(fileName, data);
        }

        public async Task SaveNewUserProfileAsync(UserProfile userProfile)
        {
            string fileName = usersProfileFileName;
            string message = userProfile.ToString() + SpecialChars.EndLine;
            byte[] data = ConversionHandler.ConvertStringToBytes(message);

            await FileStreamHandler.WriteAsync(fileName, data);
        }

        public async Task SaveNewMessageAsync(Message msg)
        {
            string fileName = messagesFileName;
            string message = msg.ToString() + SpecialChars.EndLine;
            byte[] data = ConversionHandler.ConvertStringToBytes(message);

            await FileStreamHandler.WriteAsync(fileName, data);
        }

        public async Task EmptyDataBaseAsync(string fileName)
        {
            await FileStreamHandler.EmptyFileAsync(fileName);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            string path = usersFileName;

            if (!File.Exists(path))
            {
                byte[] emptyData = ConversionHandler.ConvertStringToBytes("");
                await FileStreamHandler.WriteAsync(path, emptyData);
            }

            var fileSize = (int)FileHandler.GetFileSize(path);
            byte[] data = await FileStreamHandler.ReadAsync(path, 0, fileSize);

            string[] splitedUsers = ConversionHandler.ConvertBytesToString(data).Split(SpecialChars.EndLine);

            List<User> users = new List<User>();

            for (int i = 0; i < splitedUsers.Length; i++)
            {
                if (splitedUsers[i] != "")
                {
                    users.Add(User.ToEntity(splitedUsers[i]));
                }
            }

            return users;
        }

        public async Task<List<UserProfile>> GetAllProfilesAsync()
        {
            string path = usersProfileFileName;

            var fileSize = (int)FileHandler.GetFileSize(path);
            
            byte[] data = await FileStreamHandler.ReadAsync(path, 0, fileSize);

            string[] splitedUsersProfiles = ConversionHandler.ConvertBytesToString(data).Split(SpecialChars.EndLine);

            List<UserProfile> usersProfiles = new List<UserProfile>();

            for (int i = 0; i < splitedUsersProfiles.Length; i++)
            {
                if (splitedUsersProfiles[i] != "")
                {
                    usersProfiles.Add(UserProfile.ToEntity(splitedUsersProfiles[i]));
                }
            }

            return usersProfiles;
        }

        public async Task<List<Message>> GetAllNotReadedUserMessagesAsync(User user)
        {
            string path = messagesFileName;

            var fileSize = (int)FileHandler.GetFileSize(path);

            byte[] data = await FileStreamHandler.ReadAsync(path, 0, fileSize);

            string[] splitedMessages = ConversionHandler.ConvertBytesToString(data).Split(SpecialChars.EndLine);

            List<Message> originalMessages = new List<Message>();
            List<Message> messagesToReturn = new List<Message>();
            List<Message> newMessages = new List<Message>();

            for (int i = 0; i < splitedMessages.Length; i++)
            {
                if (splitedMessages[i] != "")
                {
                    originalMessages.Add(Message.ToEntity(splitedMessages[i]));
                }
            }

            messagesToReturn = originalMessages.FindAll(e => e.ReceiverEmail == user.Email && e.CurrentState == "" + Message.Status.NotReaded);

            for (int i = 0; i < originalMessages.Count; i++)
            {
                Message currentMessage = originalMessages[i];
                Message? newMessage = messagesToReturn.Find(e => e.Id == currentMessage.Id);

                if (newMessage != null)
                {
                    newMessage.CurrentState = "" + Message.Status.Readed;
                    newMessages.Add(newMessage);
                }
                else
                {
                    newMessages.Add(currentMessage);
                }
            }

            await EmptyDataBaseAsync(path);

            for (int i = 0; i < newMessages.Count; i++)
            {
                await SaveNewMessageAsync(newMessages[i]);
            }

            return messagesToReturn;
        }

        public async Task<List<Message>> GetUserMessagesHistoryAsync(User user)
        {
            string path = messagesFileName;

            var fileSize = (int)FileHandler.GetFileSize(path);

            byte[] data = await FileStreamHandler.ReadAsync(path, 0, fileSize);

            string[] splitedMessages = ConversionHandler.ConvertBytesToString(data).Split(SpecialChars.EndLine);

            List<Message> messages = new List<Message>();

            for (int i = 0; i < splitedMessages.Length; i++)
            {
                if (splitedMessages[i] != "")
                {
                    messages.Add(Message.ToEntity(splitedMessages[i]));
                }
            }

            messages = messages.FindAll(e => e.ReceiverEmail == user.Email);

            return messages;
        }
    }
}
