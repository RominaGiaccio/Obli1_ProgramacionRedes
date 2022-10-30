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

        public static void SendData(string fixedPart, string message, tcpServerHelper tsh)
        {
            tsh.Send(ConversionHandler.ConvertStringToBytes(fixedPart));
            tsh.Send(ConversionHandler.ConvertStringToBytes(message));
        }

        public static void SendData(string fixedPart, string message, tcpClientHelper tch)
        {
            tch.Send(ConversionHandler.ConvertStringToBytes(fixedPart));
            tch.Send(ConversionHandler.ConvertStringToBytes(message));
        }

        public static SegmentDataObject ReceiveData(tcpServerHelper tsh)
        {
            byte[] responseFixedPart = tsh.Receive(fixedPartLength);

            string stringFixedPart = ConversionHandler.ConvertBytesToString(responseFixedPart);

            string command = stringFixedPart.Substring(0, Constants.FixedCommandDataSize);
            string status = stringFixedPart.Substring(Constants.FixedCommandDataSize, Constants.FixedStatusSize);
            string dataLength = stringFixedPart.Substring(Constants.FixedCommandDataSize + Constants.FixedStatusSize, Constants.FixedDataSize);

            int dataLengthNumber = int.Parse(dataLength);

            byte[] responseData = tsh.Receive(dataLengthNumber);
            string responseMessage = Encoding.UTF8.GetString(responseData);

            return new SegmentDataObject
            {
                Command = command,
                Status = status,
                DataLength = dataLengthNumber,
                Data = responseMessage
            };
        }

        public static SegmentDataObject ReceiveData(tcpClientHelper tch)
        {
            byte[] responseFixedPart = tch.Receive(fixedPartLength);

            string stringFixedPart = ConversionHandler.ConvertBytesToString(responseFixedPart);

            string command = stringFixedPart.Substring(0, Constants.FixedCommandDataSize);
            string status = stringFixedPart.Substring(Constants.FixedCommandDataSize, Constants.FixedStatusSize);
            string dataLength = stringFixedPart.Substring(Constants.FixedCommandDataSize + Constants.FixedStatusSize, Constants.FixedDataSize);

            int dataLengthNumber = int.Parse(dataLength);

            byte[] responseData = tch.Receive(dataLengthNumber);
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
