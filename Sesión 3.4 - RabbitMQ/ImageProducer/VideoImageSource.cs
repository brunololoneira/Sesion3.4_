using OpenCvSharp;                 // Importa OpenCV para trabajar con imágenes y vídeo
using ImageLibrary;                // Importa la librería para el tipo RawImageMessage

namespace ImageProducer
{
    public class VideoImageSource : IImageSource
    {
        private VideoCapture capture;  // Captura el vídeo
        private int frameCount;        // Contador de frames procesados
        private readonly int maxFrames; // Número máximo de frames a procesar
        private readonly string videoPath; // Ruta del vídeo

        // Constructor que inicializa la captura de vídeo y establece el máximo de frames
        public VideoImageSource(string path, int maxFrames = 2)
        {
            videoPath = path;                 // Guarda la ruta del vídeo
            this.maxFrames = maxFrames;       // Establece el máximo número de frames a procesar
            capture = new VideoCapture(videoPath); // Abre el vídeo
            if (!capture.IsOpened())          // Si no se pudo abrir el vídeo, lanza una excepción
                throw new Exception($"No se pudo abrir el vídeo: {videoPath}");

            frameCount = 0;                   // Inicializa el contador de frames
        }

        // Método para adquirir un frame del vídeo
        public RawImageMessage AcquireImage()
        {
            var frame = new Mat();           // Crea un objeto para almacenar el frame del vídeo
            if (!capture.Read(frame) || frame.Empty())  // Si no se pudo leer el frame o está vacío, retorna null
                return null;

            frameCount++;                    // Incrementa el contador de frames

            // Codifica el frame como PNG y lo convierte a Base64
            Cv2.ImEncode(".png", frame, out byte[] imageBytes);
            string imageBase64 = Convert.ToBase64String(imageBytes);

            // Crea y retorna el mensaje con la imagen en Base64
            return new RawImageMessage
            {
                ImageData = imageBase64,     // Datos de la imagen en Base64
                SourceInfo = $"Frame {frameCount} de {System.IO.Path.GetFileName(videoPath)}" // Información sobre el frame
            };
        }

        // Método que indica si hay más frames en el vídeo
        public bool HasMoreImages()
        {
            return frameCount < maxFrames && capture.IsOpened(); // Retorna true si aún quedan frames
        }
    }
}
