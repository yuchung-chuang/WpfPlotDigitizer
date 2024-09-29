using System;
using System.Drawing;

namespace PlotDigitizer.Core
{
    public struct AxisTextBox
    {
        public Rectangle XMax { get; set; }
        public Rectangle YMax { get; set; }
        public Rectangle XMin { get; set; }
        public Rectangle YMin { get; set; }
        public Rectangle XLabel { get; set; }
        public Rectangle YLabel { get; set; }
    }

    public struct AxisLimitTextBoxD : IEquatable<AxisLimitTextBoxD>
    {
        public RectangleD XMax { get; set; }
        public RectangleD YMax { get; set; }
        public RectangleD XMin { get; set; }
        public RectangleD YMin { get; set; }
        public RectangleD XLabel { get; set; }
        public RectangleD YLabel { get; set; }

        public override bool Equals(object obj) => obj is AxisLimitTextBoxD d && Equals(d);
        public bool Equals(AxisLimitTextBoxD other) => XMax == other.XMax 
            && YMax == other.YMax 
            && XMin == other.XMin 
            && YMin == other.YMin 
            && XLabel == other.XLabel 
            && YLabel == other.YLabel;
        public override int GetHashCode() => HashCode.Combine(XMax, YMax, XMin, YMin, XLabel, YLabel);

        public static bool operator ==(AxisLimitTextBoxD left, AxisLimitTextBoxD right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AxisLimitTextBoxD left, AxisLimitTextBoxD right)
        {
            return !(left == right);
        }
    }
}
