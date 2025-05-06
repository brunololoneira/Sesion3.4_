using System;
using System.Diagnostics;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using ImageLibrary;

namespace ImageWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            string workerId = args.Length > 0 ? args[0] : Guid.NewGuid().ToString().Substring(0, 8);

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el exchange topic
                string exchangeName = "ImageExchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                // Declarar la cola de trabajo
                string workQueueName = "ImageWorkQueue";
                channel.QueueDeclare(queue: workQueueName,
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                // QoS - solo procesar un mensaje a la vez
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine($"[ImageWorker {workerId}] Esperando imágenes para procesar...");

                // Creamos un procesador intercambiable
                IImageProcessor processor = new FaceDetectionProcessor();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        // Deserializar la imagen recibida
                        var rawImage = MessageSerializer.Deserialize<RawImageMessage>(ea.Body.ToArray());

                        Console.WriteLine($"[ImageWorker {workerId}] Procesando imagen #{rawImage.SequenceNumber}");

                        // Procesamiento de la imagen
                        var stopwatch = Stopwatch.StartNew();
                        Thread.Sleep(2000); // Simular procesamiento que lleva tiempo

                        // Crear mensaje de resultado
                        var resultImage = processor.Process(rawImage);
                        stopwatch.Stop();

                        resultImage.SequenceNumber = rawImage.SequenceNumber;
                        resultImage.OriginalImageId = rawImage.Id.ToString();
                        resultImage.ProcessingTime = stopwatch.Elapsed;
                        resultImage.ProcessingAlgorithm = processor.GetAlgorithmName();

                        // Serializar resultado
                        var resultBody = MessageSerializer.Serialize(resultImage);

                        // Publicar resultado en Image.Result
                        channel.BasicPublish(exchange: exchangeName,
                                            routingKey: "Image.Result",
                                            basicProperties: null,
                                            body: resultBody);

                        Console.WriteLine($"[ImageWorker {workerId}] Imagen #{rawImage.SequenceNumber} procesada en {resultImage.ProcessingTime.TotalSeconds:F1}s");

                        // Confirmar procesamiento
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] {ex.Message}");
                        // Rechazar el mensaje en caso de error
                        channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                channel.BasicConsume(queue: workQueueName,
                                    autoAck: false,
                                    consumer: consumer);

                Console.WriteLine("Presiona [enter] para salir.");
                Console.ReadLine();
            }
        }
    }
}
