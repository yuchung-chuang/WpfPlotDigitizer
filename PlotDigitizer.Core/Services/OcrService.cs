using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace PlotDigitizer.Core
{
    public class OcrService : IOcrService
    {
        private readonly Tesseract ocrEngine;
        private readonly IImageService imageService;

        public OcrService(IOptions<OcrSettings> options,
            IImageService imageService)
        {
            var settings = options.Value;
            var DataPath = settings.DataPath ?? "OCR";
            var Language = settings.Language ?? "eng";
            var WhiteList = settings.WhiteList ?? "E.-0123456789";
            ocrEngine = new Tesseract(DataPath, Language, OcrEngineMode.TesseractOnly, WhiteList);
            this.imageService = imageService;
        }

        public double OcrDouble(Image<Rgba, byte> image, Rectangle textBox)
        {
            if (textBox == Rectangle.Empty) {
                return default;
            }

            var roi = image.Copy(textBox);
            ocrEngine.SetImage(roi); // Set the image for OCR
            ocrEngine.Recognize(); // Perform OCR
            var text = ocrEngine.GetUTF8Text(); // Get the recognized text
            double.TryParse(text, out var value);

            return value;
        }

        public RectangleD GetAxisLimit(Image<Rgba, byte> image, RectangleD axis)
        {
            var (xMaxTextBox, xMinTextBox, yMaxTextBox, yMinTextBox) = imageService.GetAxisLimitTextBoxes(image, axis);

            var xMax = OcrDouble(image, xMaxTextBox);
            var xMin = OcrDouble(image, xMinTextBox);
            var yMax = OcrDouble(image, yMaxTextBox);
            var yMin = OcrDouble(image, yMinTextBox);

            // combine into RectangleD
            return new RectangleD(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}
