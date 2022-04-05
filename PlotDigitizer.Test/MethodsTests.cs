﻿using PlotDigitizer.Core;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.App;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Packaging;
using System.Text;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.Core.Tests
{
	[TestClass()]
	public class MethodsTests
	{
		[TestInitialize]
		public void OnTestInitialize()
		{
			if (!UriParser.IsKnownScheme("pack"))
				new System.Windows.Application();
		}

		[DataTestMethod()]
		[DataRow("Assets/data.png", 77, 17, 759, 560)]
		[DataRow("Assets/Graph-1.jpg", 38, 43, 580, 335)]
		[DataRow("Assets/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png", 79, 0, 770, 566)]
		[DataRow("Assets/Inseam-v-Height-Graph.jpg", 48, 7, 644, 422)]
		[DataRow("Assets/rnaseqdedemo_19.png", 63, 31, 377, 343)]
		[DataRow("Assets/scatter_and_hist_border.png", 59, 222, 470, 470)]
		[DataRow("Assets/Screenshot 2021-06-26 230901.png", 104, 16, 838, 648)]
		public void GetAxisLocationTest(string uriString, int x, int y, int width, int height)
		{
			var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
			var axis = Methods.GetAxisLocation(image);
			Assert.AreEqual(new Rectangle(x, y, width, height), axis);
		}


		[DataTestMethod()]
		[DataRow("Assets/data.png", 77, 17, 759, 560)]
		[DataRow("Assets/Graph-1.jpg", 38, 43, 580, 335)]
		[DataRow("Assets/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png", 79, 0, 770, 566)]
		[DataRow("Assets/Inseam-v-Height-Graph.jpg", 48, 7, 644, 422)]
		[DataRow("Assets/rnaseqdedemo_19.png", 63, 31, 377, 343)]
		[DataRow("Assets/scatter_and_hist_border.png", 59, 222, 470, 470)]
		[DataRow("Assets/Screenshot 2021-06-26 230901.png", 104, 16, 838, 648)]
		public void CropImageTest(string uriString, int x, int y, int width, int height)
		{
			var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
			var roi = new Rectangle(x, y, width, height);
			var croppedImage = Methods.CropImage(image, roi);
			Assert.AreEqual(roi.Size, croppedImage.Size);
			Assert.AreEqual(image[roi.Y, roi.X], croppedImage[0, 0]);
			Assert.AreEqual(image[roi.Bottom - 1, roi.Right - 1], croppedImage[roi.Height - 1, roi.Width - 1]);
		}

		[TestMethod()]
		public void FilterRGBTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void GetContinuousPointsTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void GetDiscretePointsTest()
		{
			Assert.Fail();
		}

		[TestMethod()]
		public void TransformDataTest()
		{
			Assert.Fail();
		}
	}
}