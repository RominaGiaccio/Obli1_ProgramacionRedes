using GrpcServerProgram.Services;
using System.Net.Sockets;
using System.Net;
using Utils;
using Protocol;

namespace GrpcServerProgram {

    public class Program {

        public static ClientTcpHandler cth = new ClientTcpHandler();
        public static void Main(string[] args)
        {
            StartServer();
            var builder = WebApplication.CreateBuilder(args); 

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }

        public static async Task StartServer()
        {
            Console.WriteLine("Server will start accepting connections from the clients");
            await Task.Run(() => ServerInicialize());
        }

        public static async Task ServerInicialize()
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