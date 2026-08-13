using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

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

            if (!Directory.Exists(DataPath))
                return;

            try
            {
                ocrEngine = new Tesseract(DataPath, Language, OcrEngineMode.TesseractOnly, WhiteList);
            }
            catch (Exception)
            {
                // OCR is optional; a missing or invalid training-data installation should not
                // prevent the rest of the application from loading.
                ocrEngine = null;
            }
        }

        public string Ocr(Image<Rgba, byte> image)
        {
            if (ocrEngine == null)
                return string.Empty;

            try
            {
                ocrEngine.SetImage(image); // Set the image for OCR
                ocrEngine.Recognize(); // Perform OCR
                return ocrEngine.GetUTF8Text(); // Get the recognized text
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
