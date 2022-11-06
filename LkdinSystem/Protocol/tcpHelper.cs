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
            await _networkStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }

        public async Task<byte[]> ReceiveAsync(int length)
        {
            int offset = 0;
            var data = new byte[length];
            while (offset < length)
            {
                int count = length - offset;
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

