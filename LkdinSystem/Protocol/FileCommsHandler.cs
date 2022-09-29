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
        private readonly SocketHelper _socketHelper;

        public FileCommsHandler(Socket socket)
        {
            _socketHelper = new SocketHelper(socket);
        }

        public FileCommsHandler(SocketHelper socketHelper)
        {
            _socketHelper = socketHelper;
        }

        public void SendFile(string path)
        {
            if (FileHandler.FileExists(path))
            {
                var fileName = FileHandler.GetFileName(path);
                // ---> Enviar el largo del nombre del archivo
                _socketHelper.Send(ConversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                _socketHelper.Send(ConversionHandler.ConvertStringToBytes(fileName));
                // ---> Obtener el tamaño del archivo
                long fileSize = FileHandler.GetFileSize(path);
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = ConversionHandler.ConvertLongToBytes(fileSize);
                _socketHelper.Send(convertedFileSize);
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
                _socketHelper.Receive(Constants.FixedFileNameSize));
            // ---> Recibir el nombre del archivo
            string fileName = ConversionHandler.ConvertBytesToString(_socketHelper.Receive(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = ConversionHandler.ConvertBytesToLong(
                _socketHelper.Receive(Constants.FixedFileSize));
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

                _socketHelper.Send(data);
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
                    data = _socketHelper.Receive(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = _socketHelper.Receive(Constants.MaxPackageSize);
                    offset += Constants.MaxPackageSize;
                }

                FileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}

