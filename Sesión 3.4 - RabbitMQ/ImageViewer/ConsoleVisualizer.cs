using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageLibrary;

namespace ImageViewer
{
    // Implementación de ejemplo para visualizador
    class ConsoleVisualizer : IImageVisualizer
    {
        public void DisplayRawImage(RawImageMessage image)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[IMAGEN ORIGINAL #{image.SequenceNumber}] {image.ImageData}");
            Console.ResetColor();
        }

        public void DisplayProcessedImage(ProcessedImageMessage image)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[IMAGEN PROCESADA #{image.SequenceNumber}] {image.ImageData}");
            Console.WriteLine($"  Procesada con: {image.ProcessingAlgorithm} en {image.ProcessingTime.TotalSeconds:F1}s");
            Console.ResetColor();
        }
    }
}
