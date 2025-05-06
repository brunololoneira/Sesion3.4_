using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLibrary;

namespace ImageProducer
{
    // Implementación de ejemplo para la fuente de imágenes
    class SimulatedImageSource : IImageSource
    {
        private int counter = 0;
        private readonly int maxImages = 20;

        public RawImageMessage AcquireImage()
        {
            counter++;
            return new RawImageMessage
            {
                ImageData = $"Datos simulados de imagen #{counter}",
                SourceInfo = "Cámara Simulada"
            };
        }

        public bool HasMoreImages()
        {
            return counter < maxImages;
        }
    }
}
