using Enums;
using Protocol.Commands;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServerApp
{
    public class ClientHandler
    {
        public void Handler(Socket clientSocket, int number)
        {
            var sh = new SocketHelper(clientSocket);

            try
            {
                Console.WriteLine("Atention Routine");
                bool clientConnected = true;

                while (clientConnected)
                {
                    TransferSegmentManager.SegmentDataObject segment;

                    try
                    {
                        segment = TransferSegmentManager.ReceiveData(sh);
                    }
                    catch (Exception ex)
                    {
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        break;
                    }

                    Console.WriteLine("<- Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, segment.Status, segment.Data);

                    CommandsHandler.Commands.TryGetValue(segment.Command, out var commandsHandler);

                    string responseMessage = "";

                    if (commandsHandler != null)
                    {
                        try
                        {
                            if (segment.Command == "01" || segment.Command == "02" || segment.Command == "05" || segment.Command == "06" || segment.Command == "07")
                            {
                                lock (this)
                                {
                                    responseMessage = (string)commandsHandler.DynamicInvoke(segment.Data);
                                }
                            }
                            else if (segment.Command == "03" || segment.Command == "04")
                            {
                                lock (this)
                                {
                                    responseMessage = (string)commandsHandler.DynamicInvoke(segment.Data);
                                }
                            }
                            else
                            {
                                responseMessage = (string)commandsHandler.DynamicInvoke(segment.Data);
                            }

                            if (!string.IsNullOrWhiteSpace(responseMessage))
                            {
                                if (segment.Command == "03")
                                {
                                    Console.WriteLine("Reading file");
                                    var fileCommonHandler = new FileCommsHandler(clientSocket);
                                    fileCommonHandler.ReceiveFile();
                                    Console.WriteLine("Profile image received");
                                }

                                if (segment.Command == "09")
                                {
                                    Console.WriteLine("Sending image...");
                                    var fileCommonHandler = new FileCommsHandler(sh);
                                    fileCommonHandler.SendFile(responseMessage);
                                    Console.WriteLine("Image sended");
                                }
                                else
                                {
                                    string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.OK, responseMessage);

                                    TransferSegmentManager.SendData(fixedPart, responseMessage, sh);

                                    Console.WriteLine("-> Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, (int)States.OK, responseMessage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.ERROR, ex.InnerException.Message);

                            TransferSegmentManager.SendData(fixedPart, ex.InnerException.Message, sh);

                            Console.WriteLine("-> Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, (int)States.ERROR, ex.InnerException.Message);
                        }
                    }
                }
                Console.WriteLine("Client disconnect");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client disconnect - " + e.Message);
            }
        }
    }
}
