using Domain;
using Protocol;
using Protocol.Commands;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ClientApp
{
    class Program
    {
        const string localhost = "127.0.0.1";

        public static void SendProfileImage(UserProfile usu, SocketHelper clientSocket)
        {
            Console.WriteLine("Ingrese ruta de archivo");
            string absPath = Console.ReadLine() ?? "";

            usu.Image = absPath;

            ClientCommands.UploadUserProfileImage(usu, absPath, clientSocket);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Client initialize...");

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var localEndpoint = new IPEndPoint(IPAddress.Parse(localhost), 0); // Puerto 0, toma el primero disponible
            clientSocket.Bind(localEndpoint); // vinculo el socket al endpoint

            var serverEndpoint = new IPEndPoint(IPAddress.Parse(localhost), 5000);

            clientSocket.Connect(serverEndpoint);

            Console.WriteLine("Connection succesfull");

            Console.WriteLine("Write message and press enter to send it");

            bool stop = false;

            bool start = true;
            var sh = new SocketHelper(clientSocket);

            while (!stop)
            {
                if (start)
                {
                    User usu = new User("test", "test", "" + User.Status.Logged);

                    ClientCommands.CreateNewUser(usu, sh);

                    ClientCommands.SignIn(usu, sh);

                    ClientCommands.SignOut(usu, sh);

                    ClientCommands.SignIn(usu, sh);

                    string[] a = new string[2]
                    {
                        "ability 01",
                        "ability 02"
                    };

                    UserProfile up = new UserProfile(usu.Id, "some description", a, "");

                    ClientCommands.CreateUserProfile(up, sh);

                    ClientCommands.GetAllProfiles("2ff13762-6fe7-4bd0-8903-959db32d0f1d", " description", new string[] { "ability 02" }, sh);

                    var msg = new Message(usu.Email, usu.Email, "Some message content", "" + Message.Status.NotReaded);

                    ClientCommands.SendMessage(msg, sh);

                    ClientCommands.GetUnreadedMessages(usu, sh);

                    ClientCommands.GetMessagesHistory(usu, sh);

                    //SendProfileImage(up, sh);

                    //ClientCommands.DownloadUserProfileImage(up, sh);

                    start = false;
                }

                //string message = Console.ReadLine() ?? "";

                //bool isMessageEqualsToExit = message.Equals("Exit", StringComparison.InvariantCultureIgnoreCase);

                //if (isMessageEqualsToExit)
                //{
                //    stop = true;
                //} else
                //{
                //    byte[] data = Encoding.UTF8.GetBytes(message);
                //    byte[] dataLength = BitConverter.GetBytes(data.Length);

                //    int offset = 0;
                //    int size = Constants.FixedDataSize;

                //    while (offset < size)
                //    {
                //        int sended = clientSocket.Send(dataLength, offset, size - offset, SocketFlags.None);

                //        if (sended == 0)
                //        {
                //            throw new SocketException();
                //        }

                //        offset += sended;
                //    }

                //    offset = 0;
                //    size = data.Length;

                //    while (offset < size)
                //    {
                //        int sended = clientSocket.Send(data, offset, size - offset, SocketFlags.None);

                //        if (sended == 0)
                //        {
                //            throw new SocketException();
                //        }

                //        offset += sended;
                //    }

                // receive response
                //byte[] dataResponseLength = new byte[2];
                //int request = clientSocket.Receive(dataResponseLength);

                //try
                //{
                //    if (request == 0)
                //    {
                //        throw new SocketException();
                //    }

                //    byte[] dataResponse = new byte[2];
                //    request = clientSocket.Receive(dataResponse);

                //    if (request == 0)
                //    {
                //        throw new SocketException();
                //    }

                //    string responseMessage = Encoding.UTF8.GetString(dataResponse);

                //    Console.WriteLine("Server response: {0}", responseMessage);
                //}
                //catch (SocketException e)
                //{
                //    Console.WriteLine("Connection Lost: {0}", e.Message);
                //}
                //}
            }

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}