using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TopicConsumer
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
                string exchangeName = "LogsTopic";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                // Crear una cola temporal
                var queueName = channel.QueueDeclare().QueueName;

                // Enlazar la cola al exchange con un patrón de routing key
                Console.WriteLine("Introduce el binding key (ej: 'alerta.*' o '#'):");
                string bindingKey = Console.ReadLine();
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: bindingKey);

                Console.WriteLine($"Esperando mensajes Topic con binding key '{bindingKey}'. Presiona cualquier tecla para salir.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[Consumer Topic] Recibido: {message}");
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.ReadKey();
            }
        }
    }
}
