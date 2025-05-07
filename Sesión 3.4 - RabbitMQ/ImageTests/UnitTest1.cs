using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using ImageLibrary;



namespace ImageLibrary.Tests{
    public class RawImageMessage
    {
        public int SequenceNumber { get; set; }
        public string ImageData { get; set; }
        public string SourceInfo { get; set; }

    }



    public class ProcessedImageMessage

    {

        public string ImageData { get; set; }

    }



    /*

    public static class MessageSerializer

    {

        public static byte[] Serialize<T>(T message)

        {

            var json = System.Text.Json.JsonSerializer.Serialize(message);

            return Encoding.UTF8.GetBytes(json);

        }

 

        public static T Deserialize<T>(byte[] data)

        {

            var json = Encoding.UTF8.GetString(data);

            return System.Text.Json.JsonSerializer.Deserialize<T>(json);

        }

    }

    */



    public interface IImageSource

    {

        RawImageMessage AcquireImage();

        bool HasMoreImages();

    }



    public interface IImageProcessor

    {

        ProcessedImageMessage Process(RawImageMessage image);

        string GetAlgorithmName();

    }



    public interface IImageVisualizer

    {

        void DisplayRawImage(RawImageMessage image);

        void DisplayProcessedImage(ProcessedImageMessage image);

    }



    // Dummy implementations for testing

    public class SimulatedImageSource : IImageSource

    {

        private int counter = 0;

        public RawImageMessage AcquireImage()

        {

            counter++;

            return new RawImageMessage

            {

                SequenceNumber = counter,

                ImageData = $"Imagen{counter}",

                SourceInfo = "Simulador"

            };

        }

        public bool HasMoreImages() => counter < 3;

    }



    public class DummyProcessor : IImageProcessor

    {

        public ProcessedImageMessage Process(RawImageMessage image)

        {

            return new ProcessedImageMessage { ImageData = "Procesada:" + image.ImageData };

        }

        public string GetAlgorithmName() => "Dummy";

    }



    public class DummyVisualizer : IImageVisualizer

    {

        public bool RawDisplayed = false;

        public bool ProcessedDisplayed = false;



        public void DisplayRawImage(RawImageMessage image) => RawDisplayed = true;

        public void DisplayProcessedImage(ProcessedImageMessage image) => ProcessedDisplayed = true;

    }



    [TestClass]

    public class PracticaTests

    {

        /*

        [TestMethod]

        public void RawImageMessage_SerializeDeserialize_Works()

        {

            var original = new RawImageMessage

            {

                SequenceNumber = 7,

                ImageData = "TestImage",

                SourceInfo = "UnitTest"

            };

 

            var bytes = MessageSerializer.Serialize(original);

            var recovered = MessageSerializer.Deserialize<RawImageMessage>(bytes);

 

            Assert.AreEqual(original.SequenceNumber, recovered.SequenceNumber);

            Assert.AreEqual(original.ImageData, recovered.ImageData);

            Assert.AreEqual(original.SourceInfo, recovered.SourceInfo);

        }

        */



        [TestMethod]

        public void SimulatedImageSource_GeneratesDifferentImages()

        {

            var source = new SimulatedImageSource();

            var img1 = source.AcquireImage();

            var img2 = source.AcquireImage();



            Assert.AreNotEqual(img1.ImageData, img2.ImageData);

            Assert.AreEqual("Simulador", img1.SourceInfo);

            Assert.AreEqual("Simulador", img2.SourceInfo);

        }



        [TestMethod]

        public void SimulatedImageSource_HasMoreImages_Limit()

        {

            var source = new SimulatedImageSource();

            for (int i = 0; i < 3; i++)

                source.AcquireImage();



            Assert.IsFalse(source.HasMoreImages());

        }



        [TestMethod]

        public void DummyProcessor_ProcessesImage()

        {

            var proc = new DummyProcessor();

            var raw = new RawImageMessage { ImageData = "data" };

            var processed = proc.Process(raw);



            Assert.IsTrue(processed.ImageData.StartsWith("Procesada:"));

            Assert.AreEqual("Dummy", proc.GetAlgorithmName());

        }



        [TestMethod]

        public void DummyVisualizer_DisplaysImages()

        {

            var vis = new DummyVisualizer();

            vis.DisplayRawImage(new RawImageMessage());

            vis.DisplayProcessedImage(new ProcessedImageMessage());



            Assert.IsTrue(vis.RawDisplayed);

            Assert.IsTrue(vis.ProcessedDisplayed);

        }

    }

}