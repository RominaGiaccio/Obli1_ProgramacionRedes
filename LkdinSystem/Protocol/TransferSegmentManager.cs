using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Protocol
{
    public class TransferSegmentManager
    {
        public static string GerFixedPart(string code, States status, string message)
        {
            int dataLength = message.Length;

            string fixedPart = code + (int)status + NumberConvertions.AddZerosAtLeft(dataLength, Constants.FixedDataSize);

            return fixedPart;
        }

        public class SegmentDataObject
        {
            public string Command { get; set; } = String.Empty;
            public string Status { get; set; } = String.Empty;
            public int DataLength { get; set; } = 0;
            public string Data { get; set; } = String.Empty;
        }

        public static readonly int fixedPartLength = Constants.FixedCommandDataSize + Constants.FixedStatusSize + Constants.FixedDataSize;

        public static void SendData(string fixedPart, string message, tcpServerHelper sh)
        {
            sh.Send(ConversionHandler.ConvertStringToBytes(fixedPart));
            sh.Send(ConversionHandler.ConvertStringToBytes(message));
        }

        public static SegmentDataObject ReceiveData(tcpServerHelper sh)
        {
            byte[] responseFixedPart = sh.Receive(fixedPartLength);

            string stringFixedPart = ConversionHandler.ConvertBytesToString(responseFixedPart);

            string command = stringFixedPart.Substring(0, Constants.FixedCommandDataSize);
            string status = stringFixedPart.Substring(Constants.FixedCommandDataSize, Constants.FixedStatusSize);
            string dataLength = stringFixedPart.Substring(Constants.FixedCommandDataSize + Constants.FixedStatusSize, Constants.FixedDataSize);

            int dataLengthNumber = int.Parse(dataLength);

            byte[] responseData = sh.Receive(dataLengthNumber);
            string responseMessage = Encoding.UTF8.GetString(responseData);

            return new SegmentDataObject
            {
                Command = command,
                Status = status,
                DataLength = dataLengthNumber,
                Data = responseMessage
            };
        }
    }
}
