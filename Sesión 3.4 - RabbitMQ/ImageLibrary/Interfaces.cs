using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Interfaces.cs
namespace ImageLibrary
{
    // Fuente de imágenes intercambiable
    public interface IImageSource
    {
        RawImageMessage AcquireImage();
        bool HasMoreImages();
    }

    // Procesador de imágenes intercambiable
    public interface IImageProcessor
    {
        ProcessedImageMessage Process(RawImageMessage rawImage);
        string GetAlgorithmName();
    }

    // Visualizador intercambiable
    public interface IImageVisualizer
    {
        void DisplayRawImage(RawImageMessage image);
        void DisplayProcessedImage(ProcessedImageMessage image);
    }
}

