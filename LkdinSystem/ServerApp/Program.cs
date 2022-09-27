using Domain;
using Enums;
using Protocol;
using Protocol.Commands;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace ServerApp
{
    class Program
    {
        public static ClientHandler ch = new ClientHandler();

        public static void Main(string[] args)
        {
            Console.WriteLine("Server initialize...");

            var serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            string ipAddress = SettingsManager.ReadSettings(ServerConfig.serverIPConfigKey);
            int ipPort = int.Parse(SettingsManager.ReadSettings(ServerConfig.serverPortConfigKey));

            var localEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), ipPort);

            serverSocket.Bind(localEndpoint); // vinculo el socket al endpoint
            serverSocket.Listen(1); // pongo el servidor en listen

            bool exit = false;
            int clientsCount = 0;

            while (!exit)
            {
                var clientSocket = serverSocket.Accept();
                clientsCount++;

                Console.WriteLine("New client {0} accepted", clientsCount);

                new Thread(() => ch.Handler(clientSocket, clientsCount)).Start();
            }

            Console.ReadLine();
        }
    }
}