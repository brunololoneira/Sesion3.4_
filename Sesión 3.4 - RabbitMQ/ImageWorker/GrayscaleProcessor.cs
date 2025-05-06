using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLibrary;

namespace ImageWorker
{
    // Implementación de ejemplo para procesador de imágenes
    class GrayscaleProcessor : IImageProcessor
    {
        public ProcessedImageMessage Process(RawImageMessage rawImage)
        {
            // Simulación de procesamiento (convertir a escala de grises)
            return new ProcessedImageMessage
            {
                ImageData = $"Imagen en escala de grises: {rawImage.ImageData}"
            };
        }

        public string GetAlgorithmName()
        {
            return "Grayscale Filter";
        }
    }
}
