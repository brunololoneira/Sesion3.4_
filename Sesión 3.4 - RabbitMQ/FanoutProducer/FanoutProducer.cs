using RabbitMQ.Client;
using System;
using System.Text;

namespace FanoutProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el intercambiador (exchange) de tipo fanout
                string exchangeName = "LogsFanout";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

                // Enviar mensajes
                for (int i = 0; i < 5; i++)
                {
                    string message = $"Mensaje Fanout {i}: {DateTime.Now}";
                    var body = Encoding.UTF8.GetBytes(message);

                    // Publicar el mensaje al exchange (sin routing key)
                    channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);
                    Console.WriteLine($"[Productor Fanout] Enviado: {message}");
                }

                Console.WriteLine("Presiona cualquier tecla para salir.");
                Console.ReadKey();
            }
        }
    }
}
