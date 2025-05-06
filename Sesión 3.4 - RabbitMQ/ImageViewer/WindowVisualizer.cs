using OpenCvSharp;
using ImageLibrary;
using System;

namespace ImageViewer
{
    class WindowVisualizer : IImageVisualizer
    {
        public void DisplayRawImage(RawImageMessage image)
        {
            ShowImageFromBase64(image.ImageData, $"Raw #{image.SequenceNumber}");
        }

        public void DisplayProcessedImage(ProcessedImageMessage image)
        {
            ShowImageFromBase64(image.ImageData, $"Procesada #{image.SequenceNumber}");
        }

        private void ShowImageFromBase64(string base64, string windowTitle)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            Mat img = Cv2.ImDecode(imageBytes, ImreadModes.Color);

            Cv2.ImShow(windowTitle, img);
            Cv2.WaitKey(1);  // Espera corta para que se vea la ventana
        }
    }
}
