using Protocol;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    class Program
    {
        const string localhost = "127.0.0.1";

        public static void Main(string[] args)
        {
            Console.WriteLine("Server initialize...");

            var serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);


            var localEndpoint = new IPEndPoint(IPAddress.Parse(localhost), 5000);

            serverSocket.Bind(localEndpoint); // vinculo el socket al endpoint
            serverSocket.Listen(1); // pongo el servidor en listen

            bool exit = false;
            int clientsCount = 0;

            while (!exit)
            {
                var clientSocket = serverSocket.Accept();
                clientsCount++;

                Console.WriteLine("New client {0} accepted", clientsCount);
                new Thread(() => ManejarCliente(clientSocket, clientsCount)).Start();
            }

            Console.ReadLine();
        }

        static void ManejarCliente(Socket clientSocket, int number)
        {
            try
            {
                Console.WriteLine("Atention Routine");
                bool clientConnected = true;

                while (clientConnected)
                {
                    byte[] dataLength = new byte[Constants.FixedLength];

                    int offset = 0;
                    int size = Constants.FixedLength;

                    while (offset < size)
                    {
                        int receive = clientSocket.Receive(dataLength, offset, size - offset, SocketFlags.None);

                        if (receive == 0)
                        {
                            throw new SocketException();
                        }

                        offset += size;
                    }

                    byte[] data = new byte[BitConverter.ToInt32(dataLength)];

                    offset = 0;
                    size = BitConverter.ToInt32(dataLength);

                    while (offset < size)
                    {
                        int receive = clientSocket.Receive(data, offset, size - offset, SocketFlags.None);

                        if (receive == 0)
                        {
                            throw new SocketException();
                        }

                        offset += size;
                    }

                    string message = Encoding.UTF8.GetString(data);

                    Console.WriteLine("Client {0} says: {1}", number, message);

                    // sending response from server
                    //string serverResponse = "OK";
                    //byte[] responseData = Encoding.UTF8.GetBytes(serverResponse);
                    //byte[] responseDataLength = BitConverter.GetBytes(data.Length);
                    //clientSocket.Send(responseDataLength);
                    //clientSocket.Send(responseData);

                }
                Console.WriteLine("Client disconnect");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client disconnect - " + e.Message);
            }
        }
    }
}