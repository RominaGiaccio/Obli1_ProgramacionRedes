using Domain;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Protocol.Commands
{
    public class ClientCommands
    {
        public static void CreateNewUser(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("01", Enums.States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void SignIn(User user, SocketHelper sh)
        {
            user.CurrentState = "" + User.Status.Logged;

            string fixedPart = TransferSegmentManager.GerFixedPart("10", Enums.States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void SignOut(User user, SocketHelper sh)
        {
            user.CurrentState = "" + User.Status.NotLogged;

            string fixedPart = TransferSegmentManager.GerFixedPart("10", Enums.States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void CreateUserProfile(UserProfile userProfile, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("02", Enums.States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void SendMessage(Message msg, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("05", Enums.States.OK, msg.ToString());
            string message = msg.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void UploadUserProfileImage(UserProfile userProfile, string path, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("03", Enums.States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            Console.WriteLine("Subiendo archivo...");

            var fileCommonHandler = new FileCommsHandler(sh);
            fileCommonHandler.SendFile(path);

            Console.WriteLine("Archivo enviado");

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void DownloadUserProfileImage(UserProfile userProfile, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("09", Enums.States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            Console.WriteLine("Recibiendo imagen de perfil");
            var fileCommonHandler = new FileCommsHandler(sh);
            fileCommonHandler.ReceiveFile();
            Console.WriteLine("Imagen de perfil recibida");
        }

        public static void GetAllProfiles(string userId, string description, string[] abilities, SocketHelper sh)
        {
            string abilitiesMessage = "";

            for (int i = 0; i < abilities.Length; i++)
            {
                abilitiesMessage += abilities[i];

                if (i != abilities.Length - 1)
                {
                    abilitiesMessage += SpecialChars.ArrayDivider;
                }
            }

            string msg = userId + SpecialChars.Separator + description + SpecialChars.Separator + abilitiesMessage;

            string fixedPart = TransferSegmentManager.GerFixedPart("04", Enums.States.OK, msg);

            TransferSegmentManager.SendData(fixedPart, msg, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            if (response.Data != "No profiles")
            {
                var profiles = response.Data.Split(SpecialChars.EndLine);

                Console.WriteLine("UserId" + SpecialChars.Separator + "Description" + SpecialChars.Separator + "Abilities" + SpecialChars.Separator + "Image");

                for (int i = 0; i < profiles.Length; i++)
                {
                    Console.WriteLine(UserProfile.ToEntity(profiles[i]));
                }
            }
            else
            {
                Console.WriteLine(response.Data);
            }
        }

        public static void GetUnreadedMessages(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("06", Enums.States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void GetMessagesHistory(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("07", Enums.States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }
    }
}
