using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ImageLibrary;

namespace ImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el exchange topic
                string exchangeName = "ImageExchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                // Declarar una cola temporal para recibir de Image.Raw
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                 exchange: exchangeName,
                                 routingKey: "Image.Raw");

                // Declarar la cola de trabajo para los Workers
                string workQueueName = "ImageWorkQueue";
                channel.QueueDeclare(queue: workQueueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                Console.WriteLine("[ImageProcessor] Esperando imágenes...");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        // Deserializar la imagen recibida
                        var message = MessageSerializer.Deserialize<RawImageMessage>(ea.Body.ToArray());

                        Console.WriteLine($"[ImageProcessor] Recibida imagen #{message.SequenceNumber}. Enviando a cola de trabajo.");

                        // Propiedades del mensaje para la cola de trabajo (persistencia)
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        properties.Headers = new System.Collections.Generic.Dictionary<string, object>
                        {
                            { "SequenceNumber", message.SequenceNumber }
                        };

                        // Enviar a la cola de trabajo
                        channel.BasicPublish(exchange: "",
                                            routingKey: workQueueName,
                                            basicProperties: properties,
                                            body: ea.Body.ToArray());

                        // Confirmar recepción
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] {ex.Message}");
                        // Rechazar el mensaje en caso de error
                        channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                channel.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);

                Console.WriteLine("Presiona [enter] para salir.");
                Console.ReadLine();
            }
        }
    }
}
