using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class FileOperations
    {
        public static long CalculateFileParts(long fileSize)
        {
            var fileParts = fileSize / Constants.MaxPackageSize;
            return fileParts * Constants.MaxPackageSize == fileSize ? fileParts : fileParts + 1;
        }
    }
}
