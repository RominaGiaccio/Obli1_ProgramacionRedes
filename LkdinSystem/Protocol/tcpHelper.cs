using System.Net.Sockets;
using System.Text;

namespace Protocol
{
    public class tcpHelper
    {
        private readonly NetworkStream _networkStream;

        public tcpHelper(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }
        public tcpHelper(TcpClient tcpClientSocket)
        {
            NetworkStream networkStream = tcpClientSocket.GetStream();
            _networkStream = networkStream;
        }

        public void Send(byte[] data)
        {
            //obtener datos a enviar
            //byte[] data = Encoding.UTF8.GetBytes(option);
            //obtener el largo de los datos del mensaje 
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            //enviar el largo 
            _networkStream.Write(dataLength, 0, Protocol.Constants.WordLength);
            //Enviar el mensaje
            _networkStream.Write(data, 0, data.Length);
        }

        public byte[] Receive(int length)
        {
            //recibo el largo del mensaje
            int offset = 0; //offset
            byte[] dataLength = new byte[length];//new byte[Protocol.Constants.WordLength];
            while (offset < Protocol.Constants.WordLength)
            {
                //para recibir o leer el buffer si o si tenemos que llevar a mano lo que se recibe
                // y cotnrolar que recibimos lo que necesitamos
                var count = Protocol.Constants.WordLength - offset;
                int recived = _networkStream.Read(dataLength, offset, count);
                if (recived == 0)
                {
                    //cerrar conexion con cliente o lanzar excepcion, enviar mensaje
                    //cerrar conexion
                    _networkStream.Close();
                    throw new Exception("Connection ends");
                    //return;
                }
                offset += recived;
            }
            var largo = BitConverter.ToInt32(dataLength);

            //recibo el mensaje
            var data = new byte[largo];
            offset = 0;
            while (offset < largo)
            {
                int count = largo - offset;
                int recived = _networkStream.Read(data, offset, count);
                if (recived == 0)
                {
                    //cerrar conexion con cliente o lanzar excepcion, enviar mensaje
                    //return;
                    throw new Exception("Connection lost");
                }

                offset += recived;
            }


            //string mensaje = Encoding.UTF8.GetString(data);

            return data;
            //Console.WriteLine("El cliente dice: {0}", mensaje);
            /*int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                int count = length - offset;
                int recived = _networkStream.Read(data, offset, count);
                if (recived == 0)
                    throw new Exception("Connection lost");
                offset += recived;
            }
            return data;*/
        }

    }
}

