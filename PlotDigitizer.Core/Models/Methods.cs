using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PlotDigitizer.Core
{
	public static class Methods
	{
		public static Rectangle? GetAxisLocation(Image<Rgba, byte> image)
		{
			var gray = new Mat();
			CvInvoke.CvtColor(image, gray, ColorConversion.Rgba2Gray);

			var binary = new Mat();
			var threshold = CvInvoke.Threshold(gray, binary, 0, 255, ThresholdType.Otsu | ThresholdType.BinaryInv);

			var rectangles = new List<Rectangle>();
			using var contours = new VectorOfVectorOfPoint();
			CvInvoke.FindContours(binary, contours, null, RetrType.List,
				ChainApproxMethod.ChainApproxSimple);
			var count = contours.Size;
			for (var i = 0; i < count; i++) {
				using var contour = contours[i];
				using var approxContour = new VectorOfPoint();
				CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, false) * 0.01, false);
				rectangles.Add(CvInvoke.BoundingRectangle(approxContour));
			}


			var filtered = rectangles.Where(r =>
				r.Width * r.Height > image.Width * image.Height * 0.25 &&
				r.Width * r.Height < image.Width * image.Height * 0.9);
			if (!filtered.Any()) {
				return null;
			}
			var maxArea = filtered.Max(r => r.Width * r.Height);
			var axis = filtered.First(r => r.Width * r.Height == maxArea);
			return axis;
		}

		public static Image<Rgba, byte> CropImage(Image<Rgba, byte> image, Rectangle roi)
		{
			roi.X = Math.Max(roi.X, 0);
			roi.Y = Math.Max(roi.Y, 0);
			roi.Width = Math.Min(roi.Width, image.Width);
			roi.Height = Math.Min(roi.Height, image.Height);
			return image.Copy(roi);
		}

		public static Image<Rgba, byte> FilterRGB(Image<Rgba, byte> image, Rgba min, Rgba max)
		{
			var mask = image.InRange(min, max);
			image = image.Copy();
			image.SetValue(new Rgba(0, 0, 0, 0), mask.Not());
			return image;
		}

		public static void EraseImage(Image<Rgba, byte> image, Rectangle rect)
		{
			CvInvoke.Rectangle(image, rect, new Rgba().MCvScalar, -1);
		}

		public static void EraseImage(Image<Rgba, byte> image, IInputArray points)
		{
			CvInvoke.FillPoly(image, points, new Rgba().MCvScalar);
		}

		public static void DrawImage(Image<Rgba, byte> image, Point centre, int radius)
		{
			CvInvoke.Circle(image, centre, radius, new Rgba(0, 0, 0, 255).MCvScalar, -1);
		}

		public static IEnumerable<PointD> GetContinuousPoints(Image<Rgba, byte> image)
		{
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

		public static IEnumerable<PointD> GetDiscretePoints(Image<Rgba, byte> image)
		{
			var points = new List<PointD>();
			var binary = image.InRange(new Rgba(0, 0, 0, 1), new Rgba(255, 255, 255, 255));
			using var contours = new VectorOfVectorOfPoint();
			CvInvoke.FindContours(binary, contours, null, RetrType.List,
				ChainApproxMethod.ChainApproxSimple);
			var count = contours.Size;
			for (var i = 0; i < count; i++) {
				using var contour = contours[i];
				var moments = CvInvoke.Moments(contour);
				var Cx = Math.Round(moments.M10 / moments.M00);
				var Cy = Math.Round(moments.M01 / moments.M00);
				points.Add(new PointD(Cx, Cy));

				CvInvoke.DrawMarker(image, new Point((int)Cx, (int)Cy), new Rgba(255, 0, 0, 255).MCvScalar, MarkerTypes.Cross, 5);
			}

			return points;
		}

		public static IEnumerable<PointD> TransformData(IEnumerable<PointD> points, Size imageSize, RectangleD axLim, PointD axLogBase)
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
}