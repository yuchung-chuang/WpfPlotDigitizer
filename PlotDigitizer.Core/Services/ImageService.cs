using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace PlotDigitizer.Core
{
#nullable enable
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService>? logger;

        public ImageService()
        {

        }
        public ImageService(ILogger<ImageService> logger)
        {
            this.logger = logger;
        }
        public RectangleD? GetAxisLocation(Image<Rgba, byte> image)
        {
            if (image is null)
                return null;

            var gray = new Image<Gray, byte>(image.Width, image.Height);
            CvInvoke.CvtColor(image, gray, ColorConversion.Rgba2Gray);

            var binary = new Image<Gray, byte>(image.Width, image.Height);
            // Assumes the background is white.
            var thresholdOtsu = CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);
            RectangleD axis = GetAxisLocation(binary);

            var thresholdManual = 225;
            if ((axis.Width < 0.1 * image.Width || axis.Height < 0.1 * image.Height)
                && thresholdOtsu < thresholdManual) {
                // try using higher threshold
                CvInvoke.Threshold(gray, binary, thresholdManual, 255, ThresholdType.BinaryInv);
                axis = GetAxisLocation(binary);
            }
            return axis;

            Rectangle GetMaxRect(Image<Gray, byte> binary)
            {
                using var contours = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(binary, contours, null, RetrType.List,
                                            ChainApproxMethod.ChainApproxSimple);

                Rectangle maxRect = default;
                var maxSize = 0;
                for (var i = 0; i < contours.Size; i++) {
                    using var contour = contours[i];
                    var rectangle = CvInvoke.BoundingRectangle(contour);

                    if (rectangle.Width * rectangle.Height > maxSize) {
                        maxRect = rectangle;
                        maxSize = rectangle.Width * rectangle.Height;
                    }
                }
                return maxRect;
            }

            RectangleD GetAxisLocation(Image<Gray, byte> binary)
            {
                binary.ClearBorder();
                var maxRect = GetMaxRect(binary);

                var L = Math.Min(maxRect.Width, maxRect.Height) / 2;
                var topLeftXY = searchTopLeft();
                var bottomRightXY = searchBottomRight();
                var bottomLeftXY = searchBottomLeft();
                var topRightXY = searchTopRight();
                var left = topLeftXY?.X ?? bottomLeftXY?.X ?? maxRect.Left;
                var top = topLeftXY?.Y ?? topRightXY?.Y ?? maxRect.Top;
                var width = (bottomRightXY?.X ?? topRightXY?.X ?? maxRect.Right) - left + 1;
                var height = (bottomRightXY?.Y ?? bottomLeftXY?.Y ?? maxRect.Bottom) - top + 1;

                return new RectangleD(left, top, width, height);

                PointD? searchTopLeft()
                {
                    for (var row = maxRect.Top; row < maxRect.Top + L; row++) {
                        for (var col = maxRect.Left; col < maxRect.Left + L; col++) {
                            if (checkRight(row, col) && checkDown(row, col)) {
                                return searchDeeper(row, col, checkRight, checkDown);
                            }
                        }
                    }
                    return null;
                }
                PointD? searchBottomRight()
                {
                    for (var row = maxRect.Bottom - 1; row > maxRect.Bottom - 1 - L; row--) {
                        for (var col = maxRect.Right - 1; col > maxRect.Right - 1 - L; col--) {
                            if (checkLeft(row, col) && checkUp(row, col)) {
                                return searchDeeper(row - 10 + 1, col - 10 + 1, checkLeft, checkUp);
                            }
                        }
                    }
                    return null;
                }
                PointD? searchBottomLeft()
                {
                    for (var row = maxRect.Bottom - 1; row > maxRect.Bottom - 1 - L; row--) {
                        for (var col = maxRect.Left; col < maxRect.Left + L; col++) {
                            if (checkRight(row, col) && checkUp(row, col)) {
                                return searchDeeper(row - 10 + 1, col, checkRight, checkUp);
                            }
                        }
                    }
                    return null;
                }
                PointD? searchTopRight()
                {
                    for (var row = maxRect.Top; row < maxRect.Top + L; row++) {
                        for (var col = maxRect.Right - 1; col > maxRect.Right - 1 - L; col--) {
                            if (checkLeft(row, col) && checkDown(row, col)) {
                                return searchDeeper(row, col - 10 + 1, checkLeft, checkDown);
                            }
                        }
                    }
                    return null;
                }

                bool checkRight(int row, int col)
                {
                    var score = L;
                    for (var i = 0; i < L; i++) {
                        if (binary.Data[row, col + i, 0] != 255) {
                            score--;
                            if (score < 0.95 * L) {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                bool checkDown(int row, int col)
                {
                    var score = L;
                    for (var i = 0; i < L; i++) {
                        if (binary.Data[row + i, col, 0] != 255) {
                            score--;
                            if (score < 0.95 * L) {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                bool checkLeft(int row, int col)
                {
                    var score = L;
                    for (var i = 0; i < L; i++) {
                        if (binary.Data[row, col - i, 0] != 255) {
                            score--;
                            if (score < 0.95 * L) {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                bool checkUp(int row, int col)
                {
                    var score = L;
                    for (var i = 0; i < L; i++) {
                        if (binary.Data[row - i, col, 0] != 255) {
                            score--;
                            if (score < 0.95 * L) {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                /// <summary>
                /// search [row:row+9] [col:col+9]
                /// </summary>
                PointD searchDeeper(int row, int col, Func<int, int, bool> check1, Func<int, int, bool> check2)
                {
                    List<PointD> candidates = [];
                    for (var c = col; c < col + 10; c++) {
                        for (var r = row; r < row + 10; r++) {
                            if (check1(r, c) && check2(r, c)) {
                                candidates.Add(new PointD(c, r));
                            }
                        }
                    }
                    var avgX = candidates.Average(p => p.X);
                    var avgY = candidates.Average(p => p.Y);
                    return new PointD(avgX, avgY);
                }
            }
        }
        public AxisLimitTextBox GetAxisLimitTextBoxes(Image<Rgba, byte> image, RectangleD axis)
        {
            // Preprocessing
            Mat gray = new();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            Mat grad = new();
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(gray, grad, MorphOp.Gradient, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            Mat bw = new();
            CvInvoke.Threshold(grad, bw, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

            Mat kernel2 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 1), new Point(-1, -1));
            Mat connected = new();
            CvInvoke.MorphologyEx(bw, connected, MorphOp.Close, kernel2, new Point(-1, -1), 1, BorderType.Default, new MCvScalar()); // Join text in close proximity

            // Find contours for text regions
            VectorOfVectorOfPoint textContours = new();
            CvInvoke.FindContours(connected, textContours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            List<Rectangle> boundingBoxes = [];
            for (int i = 0; i < textContours.Size; i++) {
                boundingBoxes.Add(CvInvoke.BoundingRectangle(textContours[i]));
            }

            // Variables to store the closest text regions
            var textBox = new AxisLimitTextBox();
            var xMaxMinDist = double.MaxValue;
            var yMaxMinDist = double.MaxValue;
            var yMinMinDist = double.MaxValue;
            var xMinMinDist = double.MaxValue;
            var xMeanMinDist = double.MaxValue;
            var yMeanMinDist = double.MaxValue;

            var topLeft = new PointD(axis.Left, axis.Top);
            var bottomRight = new PointD(axis.Right, axis.Bottom);
            var bottomLeft = new PointD(axis.Left, axis.Bottom);

            // Step 8: Iterate through the contours to find the closest text region
            for (int i = 0; i < textContours.Size; i++) {
                Rectangle textBoundingBox = CvInvoke.BoundingRectangle(textContours[i]);
                PointD textCenter = new(textBoundingBox.X + textBoundingBox.Width / 2, textBoundingBox.Y + textBoundingBox.Height / 2);

                // Calculate distance to top-left of the chart axis
                var distTopLeft = MathHelpers.Distance(textCenter, topLeft);
                if (distTopLeft < yMaxMinDist) {
                    yMaxMinDist = distTopLeft;
                    textBox.YMax = textBoundingBox;
                }

                // Calculate distance to bottom-right of the chart axis
                var distBottomRight = MathHelpers.Distance(textCenter, bottomRight);
                if (distBottomRight < xMaxMinDist) {
                    xMaxMinDist = distBottomRight;
                    textBox.XMax = textBoundingBox;
                }

                // Calculate distance to bottom-left of the chart axis
                var distBottomLeft = MathHelpers.Distance(textCenter, bottomLeft);
                if (distBottomLeft < xMinMinDist
                    && textBoundingBox.Top > axis.Bottom) {
                    xMinMinDist = distBottomLeft;
                    textBox.XMin = textBoundingBox;
                }
                if (distBottomLeft < yMinMinDist
                    && textBoundingBox.Left < axis.Left
                    && textBoundingBox.Top < axis.Bottom) {
                    yMinMinDist = distBottomLeft;
                    textBox.YMin = textBoundingBox;
                }

            }

            for (int i = 0; i < textContours.Size; i++) {
                Rectangle textBoundingBox = CvInvoke.BoundingRectangle(textContours[i]);
                PointD textCenter = new(textBoundingBox.X + textBoundingBox.Width / 2, textBoundingBox.Y + textBoundingBox.Height / 2);

                var distXMean = Math.Abs(textCenter.X - (axis.Left + axis.Width / 2));
                if (distXMean < xMeanMinDist
                    && textBoundingBox.Top > Math.Max(Math.Max(textBox.XMin.Bottom, textBox.XMax.Bottom), axis.Bottom)) {
                    xMeanMinDist = distXMean;
                    textBox.XLabel = textBoundingBox;
                }

                var distYMean = Math.Abs(textCenter.X - (axis.Top + axis.Height / 2));
                if (distYMean < yMeanMinDist) {
                    var left = axis.Left;
                    if (textBox.YMin.Left != 0) {
                        left = textBox.YMin.Left;
                    }
                    else if (textBox.YMax.Left != 0) {
                        left = textBox.YMax.Left;
                    }

                    if (textBoundingBox.Right < left) {
                        yMeanMinDist = distYMean;
                        textBox.YLabel = textBoundingBox;
                    }
                }
            }

            return textBox;
        }
        public Image<Rgba, byte> CropImage(Image<Rgba, byte> image, RectangleD roi)
        {
            return CropImage(image, new Rectangle(
                (int)Math.Round(roi.Left),
                (int)Math.Round(roi.Top),
                (int)Math.Round(roi.Width),
                (int)Math.Round(roi.Height)));
        }

        public Image<Rgba, byte> CropImage(Image<Rgba, byte> image, Rectangle roi)
        {
            if (FixROI(image, roi) is not Rectangle roiFixed) {
                return image;
            }
            try {
                return image.Copy(roiFixed);
            }
            catch (CvException ex) {
                logger?.LogError(ex.Message);
                logger?.LogError(ex.ErrorMessage);
                return image;
            }
        }

        public Rectangle? FixROI(Image<Rgba, byte> image, Rectangle roi)
        {
            if (image is null
                || roi.X >= image.Width || roi.Y >= image.Height
                || roi.Width <= 0 || roi.Height <= 0) {
                return null;
            }
            roi.X = Math.Max(roi.X, 0);
            roi.Y = Math.Max(roi.Y, 0);
            if (roi.Right > image.Width) {
                roi.Width = image.Width - roi.X;
            }
            if (roi.Bottom > image.Height) {
                roi.Height = image.Height - roi.Y;
            }
            return roi;
        }

        public Image<Rgba, byte> FilterRGB(Image<Rgba, byte> image, Rgba min, Rgba max)
        {
            if (image is null)
                return null;
            var mask = image.InRange(min, max);
            var output = image.Copy();
            output.SetValue(new Rgba(0, 0, 0, 0), mask.Not());
            return output;
        }

        public Image<Rgba, byte> ClearBorder(Image<Rgba, byte> image)
        {
            if (image is null)
                return null;
            var channels = image.Split();
            var alpha = channels[3];
            alpha.ClearBorder();
            return image.And(image, alpha);
        }

        public IEnumerable<PointD> GetContinuousPoints(Image<Rgba, byte> image)
        {
            if (image is null)
                return null;
            var points = new List<PointD>();
            var width = image.Width;
            var height = image.Height;
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    if (image.Data[y, x, 3] == 0) {
                        continue;
                    }
                    points.Add(new PointD(x, y));

                    CvInvoke.DrawMarker(image, new Point(x, y), new Rgba(255, 0, 0, 255).MCvScalar, MarkerTypes.Cross, 1);
                }
            }

            return points;
        }

        public IEnumerable<PointD> GetDiscretePoints(Image<Rgba, byte> image)
        {
            if (image is null)
                return null;
            var points = new List<PointD>();
            var binary = image.InRange(new Rgba(0, 0, 0, 1), new Rgba(255, 255, 255, 255));
            using var contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(binary, contours, null, RetrType.List,
                ChainApproxMethod.ChainApproxSimple);
            var count = contours.Size;
            for (var i = 0; i < count; i++) {
                using var contour = contours[i];
                var centroid = GetCentroid(contour);
                if (double.IsNaN(centroid.X) || double.IsNaN(centroid.Y)) {
                    centroid = GetCentroid2(contour);
                }
                points.Add(centroid);

                CvInvoke.DrawMarker(image, new Point((int)centroid.X, (int)centroid.Y), new Rgba(255, 0, 0, 255).MCvScalar, MarkerTypes.Cross, 5);
            }

            return points;

            PointD GetCentroid(VectorOfPoint contour)
            {
                if (contour.Size == 1) {
                    return new PointD(contour[0].X, contour[0].Y);
                }
                var moments = CvInvoke.Moments(contour);
                var Cx = Math.Round(moments.M10 / moments.M00);
                var Cy = Math.Round(moments.M01 / moments.M00);
                return new PointD(Cx, Cy);
            }

            PointD GetCentroid2(VectorOfPoint contour)
            {
                if (contour.Size == 1) {
                    return new PointD(contour[0].X, contour[0].Y);
                }
                var sumX = 0d;
                var sumY = 0d;
                for (int i = 0; i < contour.Size; i++) {
                    sumX += contour[i].X;
                    sumY += contour[i].Y;
                }
                return new PointD(sumX / contour.Size, sumY / contour.Size);
            }
        }

        public IEnumerable<PointD> TransformData(IEnumerable<PointD> points, Size imageSize, RectangleD axLim, PointD axLogBase)
        {
            var dataList = points.Select(pos =>
            {
                var data = new PointD
                {
                    X = LinConvert(pos.X, imageSize.Width, 0, axLim.Right, axLim.Left),
                    Y = LinConvert(imageSize.Height - pos.Y, imageSize.Height, 0, axLim.Bottom, axLim.Top),
                };

                if (axLogBase.X > 0)
                    data.X = Math.Pow(
                      axLogBase.X,
                      LinConvert(data.X, axLim.Left, axLim.Right,
                        LogBase(axLogBase.X, axLim.Left),
                        LogBase(axLogBase.X, axLim.Right))
                      );
                if (axLogBase.Y > 0)
                    data.Y = Math.Pow(
                      axLogBase.Y,
                      LinConvert(data.Y, axLim.Top, axLim.Bottom,
                        LogBase(axLogBase.Y, axLim.Top),
                        LogBase(axLogBase.Y, axLim.Bottom))
                      );
                return data;
            }).ToList();

            return dataList;

            static double LinConvert(double value1, double max1, double min1, double max2, double min2)
            {
                var r = (max2 - min2) / (max1 - min1);
                return (min2 + (value1 - min1) * r);
            }

            static double LogBase(double Base, double num)
            {
                return Math.Log(num) / Math.Log(Base);
            }
        }
    }

    /// <summary>
    /// These extension methods change the internal state of the object.
    /// </summary>
    public static class ImageExtensionMethods
    {
        /// <summary>
        /// Only works on binary image. Clear all pixels with 255 that connects to the border.
        /// </summary>
        /// <param name="image">A binary image.</param>
        public static void ClearBorder(this Image<Gray, byte> image)
        {
            // Step 1: Find contours of the binary image
            using var contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(image, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

            // Step 2: Loop through each contour and check if it touches the border
            for (int i = 0; i < contours.Size; i++) {
                bool touchesBorder = false;
                using VectorOfPoint contour = contours[i];

                // Check if any point of the contour touches the image border
                for (int j = 0; j < contour.Size; j++) {
                    Point pt = contour[j];
                    if (pt.X <= 0 || pt.Y <= 0 ||
                        pt.X >= (image.Width - 1) || pt.Y >= (image.Height - 1)) {
                        touchesBorder = true;
                        break;
                    }
                }

                // Step 3: If the contour touches the border, we exclude it
                if (touchesBorder) {
                    CvInvoke.FloodFill(image, null, contour[0], new MCvScalar(0), out var _, new MCvScalar(1), new MCvScalar(1));
                }
            }
        }

        public static void EraseImage(this Image<Rgba, byte> image, Rectangle rect)
        {
            if (image is null) return;
            CvInvoke.Rectangle(image, rect, new Rgba().MCvScalar, -1);
        }

        public static void EraseImage(this Image<Rgba, byte> image, IInputArray points)
        {
            if (image is null) return;
            if (points is null) return;
            CvInvoke.FillPoly(image, points, new Rgba().MCvScalar);
        }

        public static void DrawImage(this Image<Rgba, byte> image, Point centre, int radius)
        {
            if (image is null) return;
            CvInvoke.Circle(image, centre, radius, new Rgba(0, 0, 0, 255).MCvScalar, -1);
        }
    }
}