using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotDigitizer.App;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.Core.Tests
{
	[TestClass()]
	public class ModelTests
	{
		[TestInitialize]
		public void OnTestInitialize()
		{
			if (!UriParser.IsKnownScheme("pack"))
				new System.Windows.Application();
		}

		[TestMethod()]
		public void ModelTest()
		{
			var model = new Model
			{
				InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>()
			};
			var setting = new Setting()
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete,
			};
			model.Load(setting);
			Assert.IsTrue(model.Data.Count() == 7);
		}

		[TestMethod()]
		public void ModelTest2()
		{
			var model = new Model();
			var setting = new Setting()
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete,
			};
			model.Load(setting);
			// first load setting, then load image
			model.InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();
			Assert.IsTrue(model.Data.Count() == 7);
		}

		[TestMethod()]
		public void LoadTest()
		{
			var model = new Model();
			var setting = new Setting()
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete,
			};
			model.Load(setting);

			Assert.IsTrue(typeof(Setting).GetProperties().All(prop => 
				// need to use Equals() instead of == operator to do value comparison (as ValueType overrides the Equals() method)
				Equals(prop.GetValue(setting), prop.GetValue(model.Setting))));
		}
	}
}