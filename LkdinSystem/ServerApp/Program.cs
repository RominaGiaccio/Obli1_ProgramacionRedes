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
        //public static ClientHandler ch = new ClientHandler();
        public static ClientTcpHandler cth = new ClientTcpHandler();

        public static void Main(string[] args)
        {
            Console.WriteLine("Server initialize...");

            //var serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            string ipAddress = SettingsManager.ReadSettings(ServerConfig.serverIPConfigKey);
            int ipPort = int.Parse(SettingsManager.ReadSettings(ServerConfig.serverPortConfigKey));

            //Definimos el endpoint y le pasamos la ip
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), ipPort);

            //var localEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), ipPort);

            //serverSocket.Bind(localEndpoint); // vinculo el socket al endpoint
            // serverSocket.Listen(1); // pongo el servidor en listen
            var tcpListener = new TcpListener(ipEndPoint);

            //Ponemos el listener a escuchar (No es necesario especificar protocolo, ni address family etc)
            tcpListener.Start(100);

            bool exit = false;
            int clientsCount = 0;

            while (!exit)
            {
                //Operacion bloqueante
                var tcpClientSocket = tcpListener.AcceptTcpClient(); 
                //var tcpClientSocket = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                //var clientSocket = serverSocket.Accept();
                clientsCount++;
                Console.WriteLine("New client {0} accepted", clientsCount);

                //new Thread(() => ch.Handler(clientSocket, clientsCount)).Start();
                //var task = Task.Run(async () => await HandleClient(tcpClientSocket).ConfigureAwait(false));
                var thread = new Thread(() => cth.Handler(tcpClientSocket, clientsCount));
                thread.Start();
                //new Thread(() => HandleTcpClient(tcpClientSocket)).Start();
            }

            Console.ReadLine();
        }

        /*   private static void HandleTcpClient(TcpClient tcpClientSocket)
          {

              NetworkStream networkStream = tcpClientSocket.GetStream();


          //public static ClientHandler ch = new ClientHandler();
          // while (true) {

          //recibo el largo del mensaje
          //byte[] dataLength = new byte[Protocol.Constants.WordLength]; //buffer



          /* int totalRecived = 0; //offset
           while (totalRecived < Protocol.Constants.WordLength) {
               //para recibir o leer el buffer si o si tenemos que llevar a mano lo que se recibe
               // y cotnrolar que recibimos lo que necesitamos
               var count = Protocol.Constants.WordLength - totalRecived;
               int recived = networkStream.Read(dataLength, totalRecived,count);
               if (recived == 0) {
                   //cerrar conexion con cliente o lanzar excepcion, enviar mensaje
                   //cerrar conexion
                   networkStream.Close();
                   Console.WriteLine("conexion terminada");
                   return;
               }
               totalRecived += recived;
           }
           var largo = BitConverter.ToInt32(dataLength);


           //recibo el mensaje
           var data = new byte[largo]; 
           totalRecived = 0; 
           while (totalRecived < largo)
           {
               int count = largo - totalRecived;
               int recived = networkStream.Read(data, totalRecived, count);
               if (recived == 0)
               {
                   //cerrar conexion con cliente o lanzar excepcion, enviar mensaje
                   return;
               }

               totalRecived += recived;
           }

           string mensaje = Encoding.UTF8.GetString(data);
           Console.WriteLine("El cliente dice: {0}", mensaje);
  
        //  }

    }*/
    }
}