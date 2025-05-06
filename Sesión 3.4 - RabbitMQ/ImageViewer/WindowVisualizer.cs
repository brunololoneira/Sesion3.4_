using OpenCvSharp;               // Importa OpenCV para manipulación de imágenes
using ImageLibrary;              // Importa la librería para trabajar con RawImageMessage y ProcessedImageMessage
using System;

namespace ImageViewer
{
    class WindowVisualizer : IImageVisualizer
    {
        // Muestra una imagen cruda desde un mensaje RawImageMessage
        public void DisplayRawImage(RawImageMessage image)
        {
            ShowImageFromBase64(image.ImageData, $"Raw #{image.SequenceNumber}");
        }

        // Muestra una imagen procesada desde un mensaje ProcessedImageMessage
        public void DisplayProcessedImage(ProcessedImageMessage image)
        {
            ShowImageFromBase64(image.ImageData, $"Procesada #{image.SequenceNumber}");
        }

        // Método privado para mostrar una imagen desde una cadena base64
        private void ShowImageFromBase64(string base64, string windowTitle)
        {
            // Convierte la cadena base64 a un arreglo de bytes
            byte[] imageBytes = Convert.FromBase64String(base64);

            // Decodifica los bytes a una imagen Mat de OpenCV
            Mat img = Cv2.ImDecode(imageBytes, ImreadModes.Color);

            // Muestra la imagen en una ventana con el título dado
            Cv2.ImShow(windowTitle, img);

            // Espera corta para mostrar la ventana (1 ms)
            Cv2.WaitKey(1);
        }
    }
}
