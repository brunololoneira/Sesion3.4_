using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImageProducer;
using System;

namespace ImageLibrary.Tests
{
    [TestClass]
    public class VideoImageSourceTests
    {
        private VideoImageSource _videoImageSource;

        [TestInitialize] // Este método se ejecuta antes de cada test
        public void Setup()
        {
            // Asegúrate de tener el archivo video.mp4 en la carpeta "videos" dentro de tu proyecto
            _videoImageSource = new VideoImageSource("video.mp4");
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
            Assert.IsTrue(hasMore);  // Si el video tiene más frames, debería devolver true
        }

        [TestMethod]
        public void HasMoreImages_ShouldReturnFalseIfVideoEnds()
        {
            // Lee un frame
            _videoImageSource.AcquireImage();
            bool hasMore = _videoImageSource.HasMoreImages();
            Assert.IsFalse(hasMore);  // Si el video no tiene más frames, debería devolver false
        }
    }
}
