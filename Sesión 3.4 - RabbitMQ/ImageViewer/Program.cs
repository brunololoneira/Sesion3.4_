using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ImageLibrary;

namespace ImageViewer
{
    class Program
    {
        // Diccionario para mantener el orden correcto de visualización
        private static Dictionary<int, ProcessedImageMessage> pendingProcessedImages = new Dictionary<int, ProcessedImageMessage>();
        private static int nextImageToDisplay = 1;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el exchange topic
                string exchangeName = "ImageExchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                // Crear una cola temporal para recibir todas las imágenes (Image.*)
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                 exchange: exchangeName,
                                 routingKey: "Image.*");

                Console.WriteLine("[ImageViewer] Esperando imágenes...");

                // Crear visualizador intercambiable
                IImageVisualizer visualizer = new WindowVisualizer();

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        string routingKey = ea.RoutingKey;

                        if (routingKey == "Image.Raw")
                        {
                            // Procesar imagen sin procesar
                            var rawImage = MessageSerializer.Deserialize<RawImageMessage>(ea.Body.ToArray());
                            visualizer.DisplayRawImage(rawImage);
                        }
                        else if (routingKey == "Image.Result")
                        {
                            // Procesar imagen procesada (con solución al desordenamiento)
                            var processedImage = MessageSerializer.Deserialize<ProcessedImageMessage>(ea.Body.ToArray());

                            // Almacenar la imagen en el búfer y mostrar las imágenes en orden
                            lock (pendingProcessedImages)
                            {
                                pendingProcessedImages[processedImage.SequenceNumber] = processedImage;
                                DisplayPendingImages(visualizer);
                            }
                        }

                        // Confirmar recepción
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] {ex.Message}");
                        channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                channel.BasicConsume(queue: queueName,
                                    autoAck: false,
                                    consumer: consumer);

                Console.WriteLine("Presiona [enter] para salir.");
                Console.ReadLine();
            }
        }

        // Método para mostrar imágenes en orden
        private static void DisplayPendingImages(IImageVisualizer visualizer)
        {
            while (pendingProcessedImages.ContainsKey(nextImageToDisplay))
            {
                var image = pendingProcessedImages[nextImageToDisplay];
                visualizer.DisplayProcessedImage(image);
                pendingProcessedImages.Remove(nextImageToDisplay);
                nextImageToDisplay++;
            }
        }
    }
}
