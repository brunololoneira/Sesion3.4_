using OpenCvSharp;
using ImageLibrary;
using System;

namespace ImageWorker
{
    class FaceDetectionProcessor : IImageProcessor
    {
        private readonly CascadeClassifier faceDetector;

        public FaceDetectionProcessor()
        {
            // Carga un clasificador de caras preentrenado de OpenCV
            faceDetector = new CascadeClassifier("haarcascade_frontalface_default.xml");
            if (faceDetector.Empty())
                throw new Exception("No se pudo cargar el clasificador de caras.");
        }

        public ProcessedImageMessage Process(RawImageMessage rawImage)
        {
            // Decodifica el Base64 en una imagen OpenCV
            byte[] imageBytes = Convert.FromBase64String(rawImage.ImageData);
            Mat img = Cv2.ImDecode(imageBytes, ImreadModes.Color);

            // Detección de caras
            Rect[] faces = faceDetector.DetectMultiScale(img, 1.1, 5);
            foreach (var face in faces)
            {
                Cv2.Rectangle(img, face, Scalar.Red, 2);
            }

            // Vuelve a codificar la imagen procesada
            Cv2.ImEncode(".png", img, out byte[] processedBytes);
            string processedBase64 = Convert.ToBase64String(processedBytes);

            return new ProcessedImageMessage
            {
                ImageData = processedBase64
            };
        }

        public string GetAlgorithmName()
        {
            return "Face Detection (OpenCV)";
        }
    }
}
