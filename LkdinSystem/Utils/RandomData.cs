using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class RandomData
    {
        public static string GenerateRandomID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
