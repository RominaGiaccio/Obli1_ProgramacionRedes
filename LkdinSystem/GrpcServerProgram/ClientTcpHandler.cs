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

namespace GrpcServerProgram
{
    public class ClientTcpHandler
    {
        public async Task<Task> Handler(TcpClient tcpClientListener, int number)
        {
            var th = new tcpHelper(tcpClientListener);
            NetworkStream networkStream = tcpClientListener.GetStream();

            try
            {
                Console.WriteLine("Atention Routine");
                bool clientConnected = true;

                while (clientConnected)
                {
                    TransferSegmentManager.SegmentDataObject segment;

                    try
                    {
                        segment = await TransferSegmentManager.ReceiveDataAsync(th);
                    }
                    catch (Exception ex)
                    {
                        networkStream.Close();
                        Console.WriteLine("Closing connection...");
                        break;
                    }

                    Console.WriteLine("<- Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, segment.Status, segment.Data);

                    CommandsHandler.Commands.TryGetValue(segment.Command, out var commandsHandler);

                    string responseMessage = "";

                    if (commandsHandler != null)
                    {
                        try
                        {
                            responseMessage = await (Task<string>)commandsHandler.DynamicInvoke(segment.Data);

                            if (!string.IsNullOrWhiteSpace(responseMessage))
                            {
                                if (segment.Command == "03")
                                {
                                    Console.WriteLine("Reading file");
                                    var fileCommonHandler = new FileCommsHandler(th);
                                    await fileCommonHandler.ReceiveFileAsync();
                                    Console.WriteLine("Profile image received");
                                }

                                if (segment.Command == "09")
                                {
                                    if (!responseMessage.Equals("No image") && FileHandler.FileExists(responseMessage))
                                    {
                                        string correctPathMessage = "Enviando imagen...";
                                        string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.OK, correctPathMessage);

                                        await TransferSegmentManager.SendDataAsync(fixedPart, correctPathMessage, th);

                                        Console.WriteLine("-> Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, (int)States.OK, correctPathMessage);

                                        var fileCommonHandler = new FileCommsHandler(th);
                                        await fileCommonHandler.SendFileAsync(responseMessage);
                                        Console.WriteLine("Image sended");
                                    }
                                    else
                                    {
                                        string errorMessage = "No existe la imagen";
                                        string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.ERROR, errorMessage);

                                        await TransferSegmentManager.SendDataAsync(fixedPart, errorMessage, th);

                                        Console.WriteLine("-> Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, (int)States.ERROR, errorMessage);
                                    }
                                }
                                else
                                {
                                    string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.OK, responseMessage);

                                    await TransferSegmentManager.SendDataAsync(fixedPart, responseMessage, th);

                                    Console.WriteLine("-> Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, (int)States.OK, responseMessage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.ERROR, ex.Message);

                            await TransferSegmentManager.SendDataAsync(fixedPart, ex.Message, th);

                            Console.WriteLine("-> Client: {0} - Instruction: {1} - Status: {2} - Message: {3}", number, segment.Command, (int)States.ERROR, ex.Message);
                        }
                    }
                }
                Console.WriteLine("Client disconnect");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client disconnect - " + e.Message);
            }

            return Task.CompletedTask;
        }
    }
}
