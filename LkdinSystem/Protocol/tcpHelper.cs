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

        public async Task SendAsync(byte[] data)
        {
            //obtener datos a enviar
            //obtener el largo de los datos del mensaje 
            byte[] dataLength = BitConverter.GetBytes(data.Length);
            //enviar el largo 
            await _networkStream.WriteAsync(dataLength, 0, Protocol.Constants.WordLength).ConfigureAwait(false);
            //Enviar el mensaje
            await _networkStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        public async Task<byte[]> ReceiveAsync(int length)
        {
            //recibo el largo del mensaje
            int offset = 0; //offset
            byte[] dataLength = new byte[length];
            while (offset < Protocol.Constants.WordLength)
            {
                //para recibir o leer el buffer si o si tenemos que llevar a mano lo que se recibe
                // y cotnrolar que recibimos lo que necesitamos
                var count = Protocol.Constants.WordLength - offset;
                int recived = await _networkStream.ReadAsync(dataLength, offset, count).ConfigureAwait(false);
                if (recived == 0)
                {
                    _networkStream.Close();
                    throw new Exception("Connection ends");
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
                int recived = await _networkStream.ReadAsync(data, offset, count).ConfigureAwait(false);
                if (recived == 0)
                {
                    throw new Exception("Connection lost");
                }

                offset += recived;
            }

            return data;
        }

    }
}

