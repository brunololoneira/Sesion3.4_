using System;
using System.Threading;
using RabbitMQ.Client;
using ImageLibrary;

namespace ImageProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el intercambiador de tipo topic
                string exchangeName = "ImageExchange";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                // Crear una fuente de imágenes (implementación intercambiable)
                IImageSource imageSource = new VideoImageSource("video.mp4");

                int sequenceNumber = 0;

                // Bucle principal de producción de imágenes
                while (imageSource.HasMoreImages())
                {
                    // Adquirir imagen
                    var rawImage = imageSource.AcquireImage();
                    rawImage.SequenceNumber = ++sequenceNumber;

                    // Serializar y enviar el mensaje
                    var body = MessageSerializer.Serialize(rawImage);

                    // Publicar en el tópico Image.Raw
                    channel.BasicPublish(exchange: exchangeName,
                                        routingKey: "Image.Raw",
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine($"[ImageProducer] Enviada imagen #{rawImage.SequenceNumber}");
                    Thread.Sleep(1000); // Simular captura periódica
                }
            }
        }
    }
}
