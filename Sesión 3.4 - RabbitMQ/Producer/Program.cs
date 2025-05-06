using RabbitMQ.Client;
using System;
using System.Text;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar la cola como durable para persistencia
                string queueName = "ColaTareas";
                bool durable = true; // Para que la cola persista en el broker
                channel.QueueDeclare(queue: queueName, durable: durable, exclusive: false, autoDelete: false, arguments: null);

                // Establecer las propiedades del mensaje como persistente
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; // Para que los mensajes persistan en el broker

                for (int i = 0; i < 10; i++) // Enviar 10 mensajes como ejemplo
                {
                    // Simular diferentes tiempos de trabajo
                    int segundos = new Random().Next(1, 6);
                    string message = segundos.ToString(); // El mensaje es la duración del trabajo
                    byte[] body = Encoding.UTF8.GetBytes(message);

                    // Publicar el mensaje con la clave de enrutamiento (queueName) y propiedades persistentes
                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
                    Console.WriteLine($"[Productor] Enviado tarea: {message} (duración: {segundos} segundos)");
                }

                Console.WriteLine("Presiona cualquier tecla para salir.");
                Console.ReadKey();
            }
        }
    }
}



/*
using RabbitMQ.Client;
using System;
using System.Text;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Crear la ConnectionFactory
            var factory = new ConnectionFactory() { HostName = "localhost" }; // Ajusta si RabbitMQ no está en localhost

            // 2. Usar 'using' para asegurar la correcta liberación de recursos
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 3. Declarar la cola (asegúrate de que el consumidor use el mismo nombre)
                string queueName = "ColaAT"; // Nombre de la cola
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                // 4. Preparar el mensaje
                string message = $"Mensaje enviado a las: {DateTime.Now}";
                byte[] body = Encoding.UTF8.GetBytes(message);

                // 5. Publicar el mensaje en la cola
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                Console.WriteLine($"[Productor] Enviado: {message}");

                Console.WriteLine("Presiona cualquier tecla para salir.");
                Console.ReadKey();
            }
        }
    }
}
*/