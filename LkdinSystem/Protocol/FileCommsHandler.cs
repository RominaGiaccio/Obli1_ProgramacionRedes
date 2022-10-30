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
    public class FileCommsHandler
    {
        //private readonly SocketHelper _socketHelper;
        // private readonly NetworkStream _networkStream;
        //private readonly TcpClient _tcpClientListener;
        private readonly tcpClientHelper _tch;

    //    public FileCommsHandler(TcpClient tcpClientListener)
     //   {
     //       _tch = new tcpServerHelper(tcpClientListener); ;

            //var tch = new tcpServerHelper(tcpClientListener);
            // _networkStream = tcpClientListener.GetStream();
    //    }

        public FileCommsHandler(tcpClientHelper tch)
        {
            _tch = tch;

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
                _tch.Send(ConversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                _tch.Send(ConversionHandler.ConvertStringToBytes(fileName));
                // ---> Obtener el tamaño del archivo
                long fileSize = FileHandler.GetFileSize(path);
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = ConversionHandler.ConvertLongToBytes(fileSize);
                _tch.Send(convertedFileSize);
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
                _tch.Receive(Constants.FixedFileNameSize));
            // ---> Recibir el nombre del archivo
            string fileName = ConversionHandler.ConvertBytesToString(_tch.Receive(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = ConversionHandler.ConvertBytesToLong(
                _tch.Receive(Constants.FixedFileSize));
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

                _tch.Send(data);
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
                    data = _tch.Receive(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _tch.Receive(Constants.MaxPackageSize);
                    offset += Constants.MaxPackageSize;
                }

                FileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}

