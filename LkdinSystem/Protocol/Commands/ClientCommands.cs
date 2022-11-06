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
        public static async Task<bool> CreateNewUserAsync(User user, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("01", States.OK, user.ToString());
            string message = user.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            Console.WriteLine(response.Data);
            return response.Status == ((int)States.OK).ToString();
        }

        public static async Task<User> SignInAsync(User user, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("10", States.OK, user.ToString());
            string message = user.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            Console.WriteLine(response.Data);

            if (response.Status == ((int)States.OK).ToString()) {
                return User.ToEntity(response.Data);
            }

            return null;
        }

        public static async Task<bool> SignOutAsync(User user, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("11", States.OK, user.ToString());
            string message = user.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            Console.WriteLine(response.Data);
            return response.Status == ((int)States.OK).ToString();
        }

        public static async Task<bool> CreateUserProfileAsync(UserProfile userProfile, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("02", States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            Console.WriteLine(response.Data);

            return response.Status == ((int)States.OK).ToString();
        }

        public static async Task<bool> SendMessageAsync(Message msg, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("05", States.OK, msg.ToString());
            string message = msg.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            Console.WriteLine(response.Data);
            return response.Status == ((int)States.OK).ToString();
        }

        public static async Task<bool> UploadUserProfileImageAsync(UserProfile userProfile, string path, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("03", States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            Console.WriteLine("Subiendo archivo...");

            var fileCommonHandler = new FileCommsHandler(tch);
            await fileCommonHandler.SendFileAsync(path);

            Console.WriteLine("Archivo enviado");

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            Console.WriteLine(response.Data);

            return response.Status == ((int)States.OK).ToString();
        }

        public static async Task<bool> DownloadUserProfileImageAsync(UserProfile userProfile, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("09", States.OK, userProfile.ToString());
            string message = userProfile.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            if (response.Status == ((int)States.OK).ToString())
            {
                Console.WriteLine("Recibiendo imagen de perfil...");
                var fileCommonHandler = new FileCommsHandler(tch);
                await fileCommonHandler.ReceiveFileAsync();
                Console.WriteLine("Imagen de perfil recibida...");

                return true;
            }
            else
            {
                Console.WriteLine(response.Data);

                return false;
            }
        }

        public static async Task<bool> GetAllProfilesAsync(string userId, string description, string[] abilities, tcpHelper tch)
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

            await TransferSegmentManager.SendDataAsync(fixedPart, msg, tch);

            if (!userId.Equals(""))
            {
                Console.WriteLine("Se filtro por Id de usuario: " + userId);
                Console.WriteLine("----------------------------------------------------------------------------");
            }
            else if (!description.Equals("")) {
                Console.WriteLine("Se filtro por descripcion: " + description);
                Console.WriteLine("----------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Se filtro por habilidad: " + abilities[0]);
                Console.WriteLine("----------------------------------------------------------------------------");
            }

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            if (response.Data != EmptyStatesMessages.NoProfilesMessage)
            {
                var profiles = response.Data.Split(SpecialChars.EndLine);

                for (int i = 0; i < profiles.Length; i++)
                {
                    UserProfile up = UserProfile.ToEntity(profiles[i]);
                    string mesgProfile = UserProfile.ToProfileString(up);
                    Console.WriteLine(mesgProfile);
                    Console.WriteLine("----------------------------------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine(response.Data);
            }

            return response.Status == ((int)States.OK).ToString();
        }

        public static async Task GetUnreadedMessagesAsync(User user, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("06", States.OK, user.ToString());
            string message = user.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            string[] mesajes = response.Data.Split("Readed");
            foreach (string m in mesajes)
            {
                if (!m.Equals(""))
                {
                    Message mesg = Message.ToEntity(m + "Readed");
                    string mesgString = Message.ToMessageString(mesg);
                    Console.WriteLine(mesgString);
                    Console.WriteLine("----------------------------------------------------------------------------");
                }
            }
        }

        public static async Task GetMessagesHistoryAsync(User user, tcpHelper tch)
        {
            string fixedPart = TransferSegmentManager.GerFixedPart("07", States.OK, user.ToString());
            string message = user.ToString();

            await TransferSegmentManager.SendDataAsync(fixedPart, message, tch);

            var response = await TransferSegmentManager.ReceiveDataAsync(tch);

            string[] mesajes = response.Data.Split("Readed");
            foreach (string m in mesajes)
            {
                if (!m.Equals(""))
                {
                    Message mesg = Message.ToEntity(m + "Readed");
                    string mesgString = Message.ToMessageString(mesg);
                    Console.WriteLine(mesgString);
                    Console.WriteLine("----------------------------------------------------------------------------");
                }
            }
        }
    }
}
