using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PlotDigitizer.Core
{
    public interface IOcrService
    {
        RectangleD GetAxisLimit(Image<Rgba, byte> image, RectangleD axis);
        double OcrDouble(Image<Rgba, byte> image, Rectangle textBox);
    }
}
