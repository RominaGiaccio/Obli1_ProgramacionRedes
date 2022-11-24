using Enums;
using Protocol;
using Protocol.Commands;
using RabbitMQ.Client;
using System.Net.Sockets;
using System.Text.Json;
using Utils;
using Domain;

namespace ServerApp
{
    public class ClientTcpHandler
    {
        public static void SendLog(IModel channel, string messageToSend)
        {
            string message = JsonSerializer.Serialize(new Log()
            {
                Date = DateTime.Now,
                Message = messageToSend
            });

            var body = ConversionHandler.ConvertStringToBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "logs",
                basicProperties: null,
                body: body
            );
        }

        public async Task<Task> Handler(TcpClient tcpClientListener, int number)
        {
            var th = new tcpHelper(tcpClientListener);
            NetworkStream networkStream = tcpClientListener.GetStream();
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            string logMessage = "";

            try
            {
                channel.QueueDeclare(
                    queue: "logs",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                logMessage = $"Client: {number} - Atention Routine";
                SendLog(channel, logMessage);
                Console.WriteLine(logMessage);

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

                        logMessage = $"Client: {number} - Closing connection...";
                        SendLog(channel, logMessage);
                        Console.WriteLine(logMessage);

                        break;
                    }

                    logMessage = $"<- Client: {number} - Instruction: {segment.Command} - Status: {segment.Status} - Message: {segment.Data}";
                    SendLog(channel, logMessage);
                    Console.WriteLine(logMessage);

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
                                    logMessage = $"Client: {number} - Reading file";
                                    SendLog(channel, logMessage);
                                    Console.WriteLine(logMessage);

                                    var fileCommonHandler = new FileCommsHandler(th);
                                    await fileCommonHandler.ReceiveFileAsync();

                                    logMessage = $"Client: {number} - Profile image received";
                                    SendLog(channel, logMessage);
                                    Console.WriteLine(logMessage);
                                }

                                if (segment.Command == "09")
                                {
                                    if (!responseMessage.Equals("No image") && FileHandler.FileExists(responseMessage))
                                    {
                                        string correctPathMessage = $"Client: {number} - Sending image...";
                                        string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.OK, correctPathMessage);

                                        await TransferSegmentManager.SendDataAsync(fixedPart, correctPathMessage, th);

                                        logMessage = $"-> Client: {number} - Instruction: {segment.Command} - Status: {(int)States.OK} - Message: {correctPathMessage}";
                                        SendLog(channel, logMessage);
                                        Console.WriteLine(logMessage);

                                        var fileCommonHandler = new FileCommsHandler(th);
                                        await fileCommonHandler.SendFileAsync(responseMessage);

                                        logMessage = $"Client: {number} - Image sended";
                                        SendLog(channel, logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                    else
                                    {
                                        string errorMessage = $"Client: {number} - Image not exists";
                                        string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.ERROR, errorMessage);

                                        await TransferSegmentManager.SendDataAsync(fixedPart, errorMessage, th);

                                        logMessage = $"-> Client: {number} - Instruction: {segment.Command} - Status: {(int)States.ERROR} - Message: {errorMessage}";
                                        SendLog(channel, logMessage);
                                        Console.WriteLine(logMessage);
                                    }
                                }
                                else
                                {
                                    string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.OK, responseMessage);

                                    await TransferSegmentManager.SendDataAsync(fixedPart, responseMessage, th);

                                    logMessage = $"-> Client: {number} - Instruction: {segment.Command} - Status: {(int)States.OK} - Message: {responseMessage}";
                                    SendLog(channel, logMessage);
                                    Console.WriteLine(logMessage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string fixedPart = TransferSegmentManager.GerFixedPart(segment.Command, States.ERROR, ex.Message);

                            await TransferSegmentManager.SendDataAsync(fixedPart, ex.Message, th);

                            logMessage = $"-> Client: {number} - Instruction: {segment.Command} - Status: {(int)States.ERROR} - Message: {ex.Message}";
                            SendLog(channel, logMessage);
                            Console.WriteLine(logMessage);
                        }
                    }
                }

                logMessage = $"Client: {number} disconnect";
                SendLog(channel, logMessage);
                Console.WriteLine(logMessage);
            }
            catch (SocketException e)
            {
                logMessage = $"Client: {number} disconnect - " + e.Message;
                SendLog(channel, logMessage);
                Console.WriteLine(logMessage);
            }

            return Task.CompletedTask;
        }
    }
}
