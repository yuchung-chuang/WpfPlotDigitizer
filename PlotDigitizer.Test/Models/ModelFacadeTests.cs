using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.App;

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.Core.Tests
{
	[TestClass()]
	public class ModelFacadeTests
	{
		private ServiceProvider provider;

		[TestInitialize]
		public void OnTestInitialize()
		{
			if (!UriParser.IsKnownScheme("pack"))
				new System.Windows.Application();

			var services = new ServiceCollection();
			services.AddSingleton<Model, ModelFacade>()
				.AddSingleton<Setting, SettingFacade>()
				.AddModelNodes();

			provider = services.BuildServiceProvider();
		}

		[TestMethod()]
		public void ModelTest()
		{
			var model = provider.GetRequiredService<Model>();
			model.InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();
			var setting = provider.GetRequiredService<Setting>();
			var settingTmp = new Setting()
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete,
			};
			setting.Load(settingTmp);

			Assert.IsTrue(model.Data.Count() == 7);
		}

		[TestMethod()]
		public void ModelTest2()
		{
			var model = provider.GetRequiredService<Model>();
			var setting = provider.GetRequiredService<Setting>();
			var settingTmp = new Setting()
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete,
			};
			setting.Load(settingTmp);

			// first load setting, then load image
			model.InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();
			Assert.IsTrue(model.Data.Count() == 7);
		}

		[TestMethod()]
		public void LoadTest()
		{
			var setting = provider.GetRequiredService<Setting>();
			var settingTmp = new Setting()
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete,
			};
			setting.Load(settingTmp);

			Assert.IsTrue(typeof(Setting).GetProperties().All(prop =>
				// need to use Equals() instead of == operator to do value comparison (as ValueType overrides the Equals() method)
				Equals(prop.GetValue(setting), prop.GetValue(settingTmp))));
		}
	}
}