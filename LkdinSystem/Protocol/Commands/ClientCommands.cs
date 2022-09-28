using Domain;
using Enums;
using Protocol.Enums;
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
        public static bool CreateNewUser(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("01", States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
            return response.Status == "" + States.OK; ;
        }

        public static User SignIn(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("10", States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
            Console.WriteLine("respuesta " +response.Status == "" + States.OK);
            if (response.Status == "" + States.OK) {
                return User.ToEntity(response.Data);
            } 
            return null;
        }

        public static void SignOut(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("11", States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static bool CreateUserProfile(UserProfile userProfile, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("02", States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);

            return response.Status == "" + States.OK;
        }

        public static bool SendMessage(Message msg, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("05", States.OK, msg.ToString());
            string message = msg.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
            return response.Status == "" + States.OK;
        }

        public static bool UploadUserProfileImage(UserProfile userProfile, string path, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("03", States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            Console.WriteLine("Subiendo archivo...");

            var fileCommonHandler = new FileCommsHandler(sh);
            fileCommonHandler.SendFile(path);

            Console.WriteLine("Archivo enviado");

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);

            return response.Status == "" + States.OK;
        }

        public static void DownloadUserProfileImage(UserProfile userProfile, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("09", States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            Console.WriteLine("Recibiendo imagen de perfil");
            var fileCommonHandler = new FileCommsHandler(sh);
            fileCommonHandler.ReceiveFile();
            Console.WriteLine("Imagen de perfil recibida");
        }

        public static bool GetAllProfiles(string userId, string description, string[] abilities, SocketHelper sh)
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

            string fixedPart = TransferSegmentManager.GerFixedPart("04", States.OK, msg);

            TransferSegmentManager.SendData(fixedPart, msg, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            if (response.Data != EmptyStatesMessages.NoProfilesMessage)
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

            return response.Status == "" + States.OK;
        }

        public static void GetUnreadedMessages(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("06", States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }

        public static void GetMessagesHistory(User user, SocketHelper sh)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("07", States.OK, user.ToString());
            string message = user.ToString();

            TransferSegmentManager.SendData(fixedPart, message, sh);

            var response = TransferSegmentManager.ReceiveData(sh);

            Console.WriteLine(response.Data);
        }
    }
}
