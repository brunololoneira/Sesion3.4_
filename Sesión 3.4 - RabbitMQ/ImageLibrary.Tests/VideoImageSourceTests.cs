using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageProducer;
using System;

namespace ImageLibrary.Tests
{
    [TestClass]
    public class VideoImageSourceTests
    {
        private VideoImageSource _videoImageSource;

        [TestInitialize] // Se ejecuta antes de cada test
        public void Setup()
        {
            // Asume que tienes un video.mp4 en la carpeta "videos" de tu proyecto
            _videoImageSource = new VideoImageSource("videos/video.mp4");
        }

        [TestMethod]
        public void AcquireImage_ShouldReturnValidRawImageMessage()
        {
            var image = _videoImageSource.AcquireImage();
            Assert.IsNotNull(image);  // Verifica que la imagen no es nula
            Assert.IsTrue(image.ImageData.Length > 0);  // Verifica que la imagen tiene datos (base64)
        }

        [TestMethod]
        public void HasMoreImages_ShouldReturnTrueIfVideoHasFramesLeft()
        {
            bool hasMore = _videoImageSource.HasMoreImages();
            Assert.IsTrue(hasMore);  // Si el video tiene frames, debería devolver true
        }

        [TestMethod]
        public void HasMoreImages_ShouldReturnFalseIfVideoEnds()
        {
            // Esto depende de cuántos frames tenga el video. Supón que el video tiene más de un frame.
            _videoImageSource.AcquireImage();  // Lee un frame
            bool hasMore = _videoImageSource.HasMoreImages();
            Assert.IsFalse(hasMore);  // Verifica que si el video no tiene más frames, devuelve false
        }
    }
}
