using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declarar la cola como durable (debe coincidir con el productor)
                string queueName = "ColaTareas";
                bool durable = true; // Para que la cola persista en el broker
                channel.QueueDeclare(queue: queueName, durable: durable, exclusive: false, autoDelete: false, arguments: null);

                // Establecer la calidad de servicio (QoS)
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false); // Solo 1 mensaje a la vez por consumidor

                Console.WriteLine("Esperando tareas. Presiona cualquier tecla para salir.");

                // Crear el consumidor
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    // Obtener el mensaje
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    int segundos = int.Parse(message);

                    Console.WriteLine($"[Consumer] Recibido tarea: {message} (duración: {segundos} segundos)");

                    // Simular el trabajo
                    Thread.Sleep(segundos * 1000);

                    Console.WriteLine($"[Consumer] Terminado tarea: {message}");

                    // Enviar la confirmación (ACK)
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                // Iniciar el consumo (autoAck en false)
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                Console.ReadKey();
            }
        }
    }
}



/*
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
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
                // 3. Declarar la cola (debe coincidir con el nombre del productor)
                string queueName = "ColaAT"; // Nombre de la cola
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                // 4. Crear un consumidor
                var consumer = new EventingBasicConsumer(channel);

                // 5. Asignar el manejador de eventos para cuando se reciben mensajes
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[Consumer] Recibido: {message}");
                };

                // 6. Iniciar el consumo de mensajes
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                Console.WriteLine("Esperando mensajes. Presiona cualquier tecla para salir.");
                Console.ReadKey();
            }
        }
    }
}
*/
