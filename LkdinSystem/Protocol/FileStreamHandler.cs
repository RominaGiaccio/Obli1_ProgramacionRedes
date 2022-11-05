using System.Runtime.CompilerServices;
using Utils;

namespace Protocol
{
    public class FileStreamHandler
    {
        public static async Task<byte[]> ReadAsync(string path, long offset, int length)
        {
            if (FileHandler.FileExists(path))
            {
                var data = new byte[length];

                using var fs = new FileStream(path, FileMode.Open) { Position = offset };
                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = await fs.ReadAsync(data, bytesRead, length - bytesRead);
                    if (read == 0)
                        throw new Exception("Error reading file");
                    bytesRead += read;
                }

                return data;
            }

            throw new Exception("File does not exist");
        }

        public static async Task WriteAsync(string fileName, byte[] data)
        {
            var fileMode = FileHandler.FileExists(fileName) ? FileMode.Append : FileMode.Create;
            using var fs = new FileStream(fileName, fileMode);
            await fs.WriteAsync(data, 0, data.Length);
        }

        public static async Task EmptyFileAsync(string fileName)
        {
            byte[] data = ConversionHandler.ConvertStringToBytes("");
            var fileMode = FileMode.Create;
            using var fs = new FileStream(fileName, fileMode);
            await fs.WriteAsync(data, 0, data.Length);
        }
    }
}
