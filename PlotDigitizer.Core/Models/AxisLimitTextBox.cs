﻿using System.Drawing;

namespace PlotDigitizer.Core
{
    public class AxisLimitTextBox
    {
        public Rectangle XMax { get; set; }
        public Rectangle YMax { get; set; }
        public Rectangle XMin { get; set; }
        public Rectangle YMin { get; set; }
        public Rectangle XLabel { get; set; }
        public Rectangle YLabel { get; set; }
    }

    public class AxisLimitTextBoxD
    {
        public RectangleD XMax { get; set; }
        public RectangleD YMax { get; set; }
        public RectangleD XMin { get; set; }
        public RectangleD YMin { get; set; }
        public RectangleD XLabel { get; set; }
        public RectangleD YLabel { get; set; }
    }
}
