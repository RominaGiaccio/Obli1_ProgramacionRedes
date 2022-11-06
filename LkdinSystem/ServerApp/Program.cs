using Domain;
using Enums;
using Protocol;
using Protocol.Commands;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Utils;
using System.Threading.Tasks;
using System.Net.Http;

namespace ServerApp
{
    class Program
    {
        public static ClientTcpHandler cth = new ClientTcpHandler();

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Server initialize...");

            string ipAddress = SettingsManager.ReadSettings(ServerConfig.serverIPConfigKey);
            int ipPort = int.Parse(SettingsManager.ReadSettings(ServerConfig.serverPortConfigKey));

            //Definimos el endpoint y le pasamos la ip
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), ipPort);
            var tcpListener = new TcpListener(ipEndPoint);

            //Ponemos el listener a escuchar (No es necesario especificar protocolo, ni address family etc)
            tcpListener.Start(100);

            bool exit = false;
            int clientsCount = 0;

            while (!exit)
            {
                //Operacion bloqueante
                var tcpClientListener = await tcpListener.AcceptTcpClientAsync(); 

                clientsCount++;
                Console.WriteLine("New client {0} accepted", clientsCount);
                var task = Task.Run(async () => await cth.Handler(tcpClientListener, clientsCount));
            }

            Console.ReadLine();
        }
    }
}