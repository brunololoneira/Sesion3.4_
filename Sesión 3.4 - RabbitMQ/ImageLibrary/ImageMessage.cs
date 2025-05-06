using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ImageMessage.cs
using System.Text.Json;

namespace ImageLibrary
{
    // Clase base para todos los mensajes
    public abstract class BaseMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int SequenceNumber { get; set; }
    }

    // Mensaje de imagen sin procesar
    public class RawImageMessage : BaseMessage
    {
        public string ImageData { get; set; }
        public string SourceInfo { get; set; }
    }

    // Mensaje de imagen procesada
    public class ProcessedImageMessage : BaseMessage
    {
        public string OriginalImageId { get; set; }
        public string ImageData { get; set; }
        public string ProcessingAlgorithm { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    // Utilidades para serializar/deserializar mensajes
    public static class MessageSerializer
    {
        public static byte[] Serialize<T>(T message) where T : BaseMessage
        {
            string jsonString = JsonSerializer.Serialize(message);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static T Deserialize<T>(byte[] data) where T : BaseMessage
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
