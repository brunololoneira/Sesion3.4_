using RabbitMQ.Client;
using System;
using System.Text;

namespace TopicProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar el exchange de tipo topic
                string exchangeName = "LogsTopic";
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

                // Enviar mensajes con diferentes routing keys
                SendMessage(channel, exchangeName, "alerta.critica", "Alerta CRÍTICA!");
                SendMessage(channel, exchangeName, "info.general", "Información general");
                SendMessage(channel, exchangeName, "alerta.aviso", "Alerta: aviso");

                Console.WriteLine("Presiona cualquier tecla para salir.");
                Console.ReadKey();
            }
        }

        static void SendMessage(IModel channel, string exchangeName, string routingKey, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
            Console.WriteLine($"[Productor Topic] Enviado a {routingKey}: {message}");
        }
    }
}
