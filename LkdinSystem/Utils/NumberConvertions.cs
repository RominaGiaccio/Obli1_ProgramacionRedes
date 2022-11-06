using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class NumberConvertions
    {
        public static string AddZerosAtLeft(int number, int zerosCount)
        {
            return number.ToString().PadLeft(zerosCount, '0');
        }
    }
}
