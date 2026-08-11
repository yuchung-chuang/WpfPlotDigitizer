using System;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Integration
{
	/// <summary>
	/// Composes the container the same way a frontend does, so a missing or mis-scoped
	/// registration fails here instead of at runtime. PlotDigitizer.Web currently omits
	/// <see cref="IImageService"/> and would not satisfy these tests.
	/// </summary>
	[TestClass]
	public class ModelCompositionTests
	{
		private ServiceProvider provider;

		[TestInitialize]
		public void OnTestInitialize()
		{
			provider = new ServiceCollection()
				.AddTransient<IImageService, EmguCvService>()
				.AddModel()
				.BuildServiceProvider();
		}

		[TestCleanup]
		public void OnTestCleanup() => provider?.Dispose();

		[TestMethod]
		[TestCategory("Integration")]
		public void AddModel_WithImageServiceRegistered_ResolvesEveryGraphNode()
		{
			var nodeTypes = new List<Type>
			{
				typeof(InputImageNode), typeof(CroppedImageNode), typeof(FilteredImageNode),
				typeof(EdittedImageNode), typeof(DataPointsNode), typeof(DataNode),
				typeof(AxisLocationNode), typeof(AxisTextBoxNode), typeof(AxisLimitNode),
				typeof(AxisTitleNode), typeof(AxisLogBaseNode), typeof(FilterMinNode),
				typeof(FilterMaxNode), typeof(DataTypeNode),
			};

			foreach (var nodeType in nodeTypes) {
				Assert.IsNotNull(provider.GetRequiredService(nodeType), $"{nodeType.Name} could not be resolved.");
			}

			Assert.IsNotNull(provider.GetRequiredService<Model>());
			Assert.IsNotNull(provider.GetRequiredService<Setting>());
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddModel_ResolvingModelTwice_ReturnsTheSameSingleton()
		{
			Assert.AreSame(provider.GetRequiredService<Model>(), provider.GetRequiredService<Model>());
			Assert.AreSame(provider.GetRequiredService<Setting>(), provider.GetRequiredService<Setting>());
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void CroppedImage_WhenAxisLocationChanges_IsRecomputedFromTheRealImageService()
		{
			var model = provider.GetRequiredService<Model>();
			var setting = provider.GetRequiredService<Setting>();

			// Loading straight from the output directory keeps the test free of WPF pack URIs.
			using var source = new Image<Rgba, byte>("Assets/data.png");
			model.InputImage = source;

			setting.AxisLocation = new RectangleD(10, 10, 100, 80);
			var first = model.CroppedImage;

			setting.AxisLocation = new RectangleD(10, 10, 120, 90);
			var second = model.CroppedImage;

			Assert.AreEqual(100, first.Width);
			Assert.AreEqual(80, first.Height);
			Assert.AreEqual(120, second.Width, "Changing AxisLocation must invalidate CroppedImage.");
			Assert.AreEqual(90, second.Height);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void PropertyOutdated_WhenUpstreamSettingChanges_IsRaisedByTheModelFacade()
		{
			var model = provider.GetRequiredService<Model>();
			var setting = provider.GetRequiredService<Setting>();

			using var source = new Image<Rgba, byte>("Assets/data.png");
			model.InputImage = source;
			setting.AxisLocation = new RectangleD(10, 10, 100, 80);
			_ = model.CroppedImage;

			var outdatedProperties = new List<string>();
			model.PropertyOutdated += (s, e) => outdatedProperties.Add(e);

			setting.AxisLocation = new RectangleD(20, 20, 60, 60);

			CollectionAssert.Contains(outdatedProperties, nameof(Model.CroppedImage),
				"The facade must relay node invalidation so bound views refresh.");
		}
	}
}
