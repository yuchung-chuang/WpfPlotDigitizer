using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace PlotDigitizer.Core
{
    public class OcrService : IOcrService
    {
        private readonly Tesseract ocrEngine;

        public OcrService([ServiceKey] string key, 
            IOptionsFactory<OcrSettings> optionsFactory)
        {
            var settings = optionsFactory.Create(key);
            var DataPath = settings.DataPath ?? "OCR";
            var Language = settings.Language ?? "eng";
            var WhiteList = settings.WhiteList ?? null;
            ocrEngine = new Tesseract(DataPath, Language, OcrEngineMode.TesseractOnly, WhiteList);
        }

        public string Ocr(Image<Rgba, byte> image)
        {
            ocrEngine.SetImage(image); // Set the image for OCR
            ocrEngine.Recognize(); // Perform OCR
            return ocrEngine.GetUTF8Text(); // Get the recognized text
        }
    }
}
