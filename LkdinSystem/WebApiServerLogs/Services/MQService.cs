using Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Utils;
using WebApiServerLogs.Repositories;

namespace WebApiServerLogs.Services
{
    public class MQService
    {
        public MQService()
        {
            // Conexión con RabbitMQ local:
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Defino la conexion

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(
                queue: "logs",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            //Defino el mecanismo de consumo
            var consumer = new EventingBasicConsumer(channel);
            //Defino el evento que sera invocado cuando llegue un mensaje 
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = ConversionHandler.ConvertBytesToString(body);
                Console.WriteLine(" [x] Received {0}", message);
                Log log = JsonSerializer.Deserialize<Log>(message);

                var logsRepository = LogsRepository.GetInstance();
                logsRepository.AddLog(log);
            };

            //"PRENDO" el consumo de mensajes
            channel.BasicConsume(queue: "logs",
                autoAck: true,
                consumer: consumer);
        }
    }
}
