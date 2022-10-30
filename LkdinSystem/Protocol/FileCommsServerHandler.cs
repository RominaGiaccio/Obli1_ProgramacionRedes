using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Protocol
{
    public class FileCommsServerHandler
    {
        //private readonly SocketHelper _socketHelper;
        // private readonly NetworkStream _networkStream;
        //private readonly TcpClient _tcpClientListener;
        private readonly tcpServerHelper _tsh; 

        //    public FileCommsHandler(TcpClient tcpClientListener)
        //   {
        //       _tch = new tcpServerHelper(tcpClientListener); ;

        //var tch = new tcpServerHelper(tcpClientListener);
        // _networkStream = tcpClientListener.GetStream();
        //    }

        public FileCommsServerHandler(tcpServerHelper tsh)
        {
            _tsh = tsh;

            //var tch = new tcpServerHelper(tcpClientListener);
            // _networkStream = tcpClientListener.GetStream();
        }

        /*  public FileCommsHandler(SocketHelper socketHelper)
          {
              _socketHelper = socketHelper;
          }*/

        public void SendFile(string path)
        {
            if (FileHandler.FileExists(path))
            {
                var fileName = FileHandler.GetFileName(path);
                // ---> Enviar el largo del nombre del archivo
                _tsh.Send(ConversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                _tsh.Send(ConversionHandler.ConvertStringToBytes(fileName));
                // ---> Obtener el tamaño del archivo
                long fileSize = FileHandler.GetFileSize(path);
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = ConversionHandler.ConvertLongToBytes(fileSize);
                _tsh.Send(convertedFileSize);
                // ---> Enviar el archivo (pero con file stream)
                SendFileWithStream(fileSize, path);
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public void ReceiveFile()
        {
            // ---> Recibir el largo del nombre del archivo
            int fileNameSize = ConversionHandler.ConvertBytesToInt(
                _tsh.Receive(Constants.FixedFileNameSize));
            // ---> Recibir el nombre del archivo
            string fileName = ConversionHandler.ConvertBytesToString(_tsh.Receive(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = ConversionHandler.ConvertBytesToLong(
                _tsh.Receive(Constants.FixedFileSize));
            // ---> Recibir el archivo
            ReceiveFileWithStreams(fileSize, fileName);
        }

        private void SendFileWithStream(long fileSize, string path)
        {
            long fileParts = FileOperations.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = FileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = FileStreamHandler.Read(path, offset, Constants.MaxPackageSize);
                    offset += Constants.MaxPackageSize;
                }

                _tsh.Send(data);
                currentPart++;
            }
        }

        private void ReceiveFileWithStreams(long fileSize, string fileName)
        {
            long fileParts = FileOperations.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = _tsh.Receive(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _tsh.Receive(Constants.MaxPackageSize);
                    offset += Constants.MaxPackageSize;
                }

                FileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}

