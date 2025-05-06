using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace FanoutConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el exchange (debe coincidir con el productor)
                string exchangeName = "LogsFanout";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

                // Crear una cola temporal
                var queueName = channel.QueueDeclare().QueueName;

                // Enlazar la cola al exchange
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

                Console.WriteLine("Esperando mensajes Fanout. Presiona cualquier tecla para salir.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[Consumer Fanout] Recibido: {message}");
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.ReadKey();
            }
        }
    }
}
