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
            int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                int count = length - offset;
                int recived = _networkStream.Read(data, offset, count);
                if (recived == 0)
                    throw new Exception("Connection lost");
                offset += recived;
            }
            return data;
        }
        
    }
}

