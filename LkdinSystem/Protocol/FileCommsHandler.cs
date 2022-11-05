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
        private readonly tcpHelper _th; 

        public FileCommsHandler(tcpHelper tch)
        {
            _th = tch;
        }

        public async Task SendFileAsync(string path)
        {
            if (FileHandler.FileExists(path))
            {
                var fileName = FileHandler.GetFileName(path);
                // ---> Enviar el largo del nombre del archivo
                await _th.SendAsync(ConversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                await _th.SendAsync(ConversionHandler.ConvertStringToBytes(fileName));
                // ---> Obtener el tamaño del archivo
                long fileSize = FileHandler.GetFileSize(path);
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = ConversionHandler.ConvertLongToBytes(fileSize);
                await _th.SendAsync(convertedFileSize);
                // ---> Enviar el archivo (pero con file stream)
                await SendFileWithStreamAsync(fileSize, path);
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public async Task ReceiveFileAsync()
        {
            // ---> Recibir el largo del nombre del archivo
            int fileNameSize = ConversionHandler.ConvertBytesToInt(
                await _th.ReceiveAsync(Constants.FixedFileNameSize));
            // ---> Recibir el nombre del archivo
            string fileName = ConversionHandler.ConvertBytesToString(await _th.ReceiveAsync(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = ConversionHandler.ConvertBytesToLong(
                await _th.ReceiveAsync(Constants.FixedFileSize));
            // ---> Recibir el archivo
            await ReceiveFileWithStreamsAsync(fileSize, fileName);
        }

        private async Task SendFileWithStreamAsync(long fileSize, string path)
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
                    data = await FileStreamHandler.ReadAsync(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await FileStreamHandler.ReadAsync(path, offset, Constants.MaxPackageSize);
                    offset += Constants.MaxPackageSize;
                }

                await _th.SendAsync(data);
                currentPart++;
            }
        }

        private async Task ReceiveFileWithStreamsAsync(long fileSize, string fileName)
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
                    data = await _th.ReceiveAsync(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = await _th.ReceiveAsync(Constants.MaxPackageSize);
                    offset += Constants.MaxPackageSize;
                }

                await FileStreamHandler.WriteAsync(fileName, data);
                currentPart++;
            }
        }
    }
}

