// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Grpc.Net.Client;

namespace GrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Cliente GRPC!!");
            using var channel = GrpcChannel.ForAddress("https://localhost:7109");
            var client = new Greeter.GreeterClient(channel);
            var replay = await client.SayHelloAsync(new HelloRequest { Name = "Romi" });
            Console.WriteLine("La respuesta es : {0}", replay);

            Console.ReadLine();
        }
    }
}