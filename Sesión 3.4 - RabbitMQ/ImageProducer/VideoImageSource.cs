using OpenCvSharp;
using ImageLibrary;

namespace ImageProducer
{
    class VideoImageSource : IImageSource
    {
        private VideoCapture capture;
        private int frameCount;
        private readonly int maxFrames;
        private readonly string videoPath;

        public VideoImageSource(string path, int maxFrames = 2)
        {
            videoPath = path;
            this.maxFrames = maxFrames;
            capture = new VideoCapture(videoPath);
            if (!capture.IsOpened())
                throw new Exception($"No se pudo abrir el vídeo: {videoPath}");

            frameCount = 0;
        }

        public RawImageMessage AcquireImage()
        {
            var frame = new Mat();
            if (!capture.Read(frame) || frame.Empty())
                return null;

            frameCount++;

            // Codifica el frame como imagen PNG y lo pasa a base64
            Cv2.ImEncode(".png", frame, out byte[] imageBytes);
            string imageBase64 = Convert.ToBase64String(imageBytes);

            return new RawImageMessage
            {
                ImageData = imageBase64,
                SourceInfo = $"Frame {frameCount} de {System.IO.Path.GetFileName(videoPath)}"
            };
        }

        public bool HasMoreImages()
        {
            return frameCount < maxFrames && capture.IsOpened();
        }
    }
}
