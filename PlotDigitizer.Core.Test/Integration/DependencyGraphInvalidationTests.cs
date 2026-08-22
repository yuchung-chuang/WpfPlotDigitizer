using System.Collections.Generic;
using System.Linq;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlotDigitizer.Core.Tests.Integration
{
	/// <summary>
	/// Exercises invalidation edge cases across the real dependency graph that
	/// <see cref="ModelCompositionTests"/> does not: cascades that cross from the
	/// <see cref="Setting"/> chain into the <see cref="Model"/> chain and back, cascades that
	/// stop where they should, and the fact that <see cref="InputImageNode"/> is a single
	/// upstream node shared by both facades. Uses the real <see cref="EmguCvService"/> so a test
	/// failure here means the production graph is actually wired wrong, not that a fake drifted
	/// from reality.
	/// </summary>
	[TestClass]
	public class DependencyGraphInvalidationTests
	{
		private ServiceProvider provider;
		private Model model;
		private Setting setting;
		private List<string> modelOutdated;
		private List<string> settingOutdated;

		[TestInitialize]
		public void OnTestInitialize()
		{
			provider = new ServiceCollection()
				.AddTransient<IImageService, EmguCvService>()
				.AddModel()
				.BuildServiceProvider();

			model = provider.GetRequiredService<Model>();
			setting = provider.GetRequiredService<Setting>();

			modelOutdated = [];
			settingOutdated = [];
			model.PropertyOutdated += (s, e) => modelOutdated.Add(e);
			setting.PropertyOutdated += (s, e) => settingOutdated.Add(e);
		}

		[TestCleanup]
		public void OnTestCleanup() => provider?.Dispose();

		// A node can be reachable from the changed value through more than one path (e.g.
		// CroppedImage depends on both InputImage and AxisLocation), so OnOutdated legitimately
		// fires more than once for the same property. Tests care which properties were
		// invalidated, not how many redundant times each cascade path re-announced it.
		private static List<string> Distinct(List<string> outdated) => outdated.Distinct().ToList();

		/// <summary>Computes the whole pipeline once and starts listening for invalidation from a clean, fully-computed state.</summary>
		private void ComputeFullPipelineOnce()
		{
			using var source = new Image<Rgba, byte>("Assets/data.png");
			model.InputImage = source;
			setting.AxisLocation = new RectangleD(10, 10, 100, 80);

			_ = model.Data; // pulls CroppedImage -> FilteredImage -> EditedImage -> DataPoints -> Data into a computed state

			modelOutdated.Clear();
			settingOutdated.Clear();
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void FullPipeline_FromInputImageThroughData_ProducesNonNullResultAtEveryStage()
		{
			using var source = new Image<Rgba, byte>("Assets/data.png");
			model.InputImage = source;
			setting.AxisLocation = new RectangleD(10, 10, 100, 80);

			Assert.IsNotNull(model.CroppedImage);
			Assert.IsNotNull(model.FilteredImage);
			Assert.IsNotNull(model.EditedImage);
			Assert.IsNotNull(model.DataPoints);
			Assert.IsNotNull(model.Data);
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AxisLocationChange_CascadesAcrossBothTheSettingChainAndTheFullModelChain()
		{
			ComputeFullPipelineOnce();

			setting.AxisLocation = new RectangleD(20, 20, 60, 60);

			CollectionAssert.AreEquivalent(
				new[] { nameof(Setting.AxisTextBox), nameof(Setting.AxisLimit), nameof(Setting.AxisTitle) },
				Distinct(settingOutdated),
				"AxisLocation must cascade through AxisTextBox to AxisLimit and AxisTitle.");

			CollectionAssert.AreEquivalent(
				new[] { nameof(Model.CroppedImage), nameof(Model.FilteredImage), nameof(Model.EditedImage), nameof(Model.DataPoints), nameof(Model.Data) },
				Distinct(modelOutdated),
				"AxisLocation must cascade through the entire Model chain via CroppedImage.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void FilterMinChange_OnlyCascadesFromFilteredImageOnward()
		{
			ComputeFullPipelineOnce();

			setting.FilterMin = new Rgba(10, 10, 10, byte.MaxValue);

			CollectionAssert.DoesNotContain(modelOutdated, nameof(Model.CroppedImage),
				"FilterMin does not feed CroppedImage, so it must not be invalidated.");
			CollectionAssert.AreEquivalent(
				new[] { nameof(Model.FilteredImage), nameof(Model.EditedImage), nameof(Model.DataPoints), nameof(Model.Data) },
				Distinct(modelOutdated));
			Assert.AreEqual(0, settingOutdated.Count, "FilterMin has no Setting-side dependents.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void FilterMaxChange_OnlyCascadesFromFilteredImageOnward()
		{
			ComputeFullPipelineOnce();

			setting.FilterMax = new Rgba(200, 200, 200, byte.MaxValue);

			CollectionAssert.DoesNotContain(modelOutdated, nameof(Model.CroppedImage));
			CollectionAssert.AreEquivalent(
				new[] { nameof(Model.FilteredImage), nameof(Model.EditedImage), nameof(Model.DataPoints), nameof(Model.Data) },
				Distinct(modelOutdated));
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void DataTypeChange_OnlyCascadesDataPointsAndData()
		{
			ComputeFullPipelineOnce();

			setting.DataType = setting.DataType == DataType.Continuous ? DataType.Discrete : DataType.Continuous;

			CollectionAssert.AreEquivalent(new[] { nameof(Model.DataPoints), nameof(Model.Data) }, Distinct(modelOutdated),
				"DataType must not invalidate the three upstream image stages.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void DirectEditedImageAssignment_OnlyCascadesDataPointsAndData_NeverBackUpstream()
		{
			ComputeFullPipelineOnce();

			using var edited = model.EditedImage.Copy();
			model.EditedImage = edited; // exactly what EditPageViewModel.Leave() does

			CollectionAssert.AreEquivalent(new[] { nameof(Model.DataPoints), nameof(Model.Data) }, Distinct(modelOutdated),
				"Assigning EditedImage directly must not reach back into FilteredImage or CroppedImage.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AxisLogBaseChange_OnlyCascadesData()
		{
			ComputeFullPipelineOnce();

			setting.AxisLogBase = new PointD(10, 10);

			CollectionAssert.AreEquivalent(new[] { nameof(Model.Data) }, Distinct(modelOutdated));
			Assert.AreEqual(0, settingOutdated.Count, "Nothing in the Setting chain depends on AxisLogBase.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AxisTitleChange_DoesNotInvalidateAnyOtherProperty()
		{
			ComputeFullPipelineOnce();

			setting.AxisTitle = new AxisTitle("X", "Y");

			Assert.AreEqual(0, modelOutdated.Count, "Nothing in the Model chain depends on AxisTitle.");
			Assert.AreEqual(0, settingOutdated.Count, "Nothing else in the Setting chain depends on AxisTitle.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void InputImageReplacement_CascadesAcrossBothFacadesBecauseInputImageNodeIsShared()
		{
			ComputeFullPipelineOnce();

			using var replacement = new Image<Rgba, byte>("Assets/data.png");
			model.InputImage = replacement;

			// InputImageNode is a singleton dependency of both graphs: AxisLocationNode,
			// FilterMinNode, FilterMaxNode, DataTypeNode and AxisLogBaseNode all depend on it
			// directly, and AxisLocationNode's own cascade reaches AxisTextBox/AxisLimit/AxisTitle.
			CollectionAssert.AreEquivalent(
				new[]
				{
					nameof(Setting.AxisLocation), nameof(Setting.AxisTextBox), nameof(Setting.AxisLimit), nameof(Setting.AxisTitle),
					nameof(Setting.FilterMin), nameof(Setting.FilterMax), nameof(Setting.DataType), nameof(Setting.AxisLogBase),
				},
				Distinct(settingOutdated),
				"Replacing InputImage must cascade into every Setting node that depends on it, directly or transitively.");

			CollectionAssert.AreEquivalent(
				new[] { nameof(Model.CroppedImage), nameof(Model.FilteredImage), nameof(Model.EditedImage), nameof(Model.DataPoints), nameof(Model.Data) },
				Distinct(modelOutdated));
		}
	}
}
