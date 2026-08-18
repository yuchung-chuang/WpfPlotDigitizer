using System;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

namespace PlotDigitizer.Core.Tests.Integration
{
	/// <summary>
	/// Mirrors <c>PlotDigitizer.WPF.App.OnStartup</c>'s exact <see cref="IServiceCollection"/>
	/// registrations (minus the concrete logging *providers*, which touch the file system and add
	/// nothing to a composition check — <see cref="AddLogging"/> with no provider is still
	/// included, because leaving it out entirely changes resolution behaviour; see
	/// <see cref="AddWpfServices_WithoutAddLogging_SilentlyFallsBackToParameterlessConstructors"/>).
	/// An empty in-memory <see cref="IConfiguration"/> stands in for the optional
	/// <c>appsettings.json</c> — <see cref="OcrService"/> tolerates a missing OCR data directory,
	/// so the keyed OCR services still resolve.
	/// </summary>
	[TestClass]
	public class WpfCompositionTests
	{
		private ServiceProvider provider;

		[TestInitialize]
		public void OnTestInitialize()
		{
			provider = BuildProvider(withLogging: true);
		}

		[TestCleanup]
		public void OnTestCleanup() => provider?.Dispose();

		private static ServiceProvider BuildProvider(bool withLogging)
		{
			IConfiguration configuration = new ConfigurationBuilder().Build();

			var services = new ServiceCollection()
				.AddSingleton(configuration)
				.AddOcrServices(configuration)
				.AddTransient<IImageService, EmguCvService>()
				.AddTransient<IDownloadService, DownloadService>()
				.AddTransient<IMessageBoxService, MessageBoxService>()
				.AddTransient<IFileDialogService, FileDialogService>()
				.AddTransient<IAwaitTaskService, AwaitTaskService>()
				.AddTransient<IClipboardService, ClipboardService>()
				.AddSingleton<IPageService, PageService>()
				.AddTransient<IWindowService, WindowService>()
				.AddViewModels()
				.AddModel();

			if (withLogging) {
				// App.xaml.cs always calls AddLogging(...). No provider is registered here (so
				// nothing is written to disk), but ILogger<T> must still resolve to the real
				// Logger<T> - see the regression test below for why this matters.
				services.AddLogging();
			}

			return services.BuildServiceProvider();
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_ComposedLikeApp_ResolvesEveryPageViewModelWiredToTheSharedModel()
		{
			var model = provider.GetRequiredService<Model>();

			foreach (var pageType in PageService.Pages) {
				var page = provider.GetRequiredService(pageType);
				Assert.IsNotNull(page, $"{pageType.Name} could not be resolved.");

				// Resolving the type is not enough: if the DI-facing constructor's ILogger<T>
				// can't be satisfied, ActivatorUtilities silently falls back to the parameterless
				// constructor and every dependency - including Model - stays null. Reading Model
				// back through the page's own property proves the real constructor ran.
				var modelProperty = pageType.GetProperty(nameof(Model));
				Assert.IsNotNull(modelProperty, $"{pageType.Name} is expected to expose a Model property.");
				Assert.AreSame(model, modelProperty.GetValue(page),
					$"{pageType.Name} was constructed without the shared Model - its DI constructor did not run.");
			}

			var mainViewModel = provider.GetRequiredService<MainViewModel>();
			Assert.AreSame(model, mainViewModel.Model, "MainViewModel was constructed without the shared Model.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_WithoutAddLogging_SilentlyFallsBackToParameterlessConstructors()
		{
			// This does not exercise a defect in PlotDigitizer - it documents why the composition
			// tests (and App.xaml.cs itself) must always call AddLogging(): every page view model
			// has a parameterless constructor for unit testing (per core.instructions.md), and
			// .NET's ActivatorUtilities picks it silently when ILogger<T> has no registration,
			// instead of throwing. That leaves Model/EditService/etc. null with no error raised
			// anywhere - the single riskiest failure mode a composition test can miss.
			// Deliberately not disposed: EditPageViewModel.Dispose() unsubscribes from
			// EditService.PropertyChanged, which throws a NullReferenceException here for the
			// exact reason under test (EditService was never wired up). Disposing this
			// intentionally-broken container would just rediscover the same defect via a
			// different symptom.
			var providerWithoutLogging = BuildProvider(withLogging: false);

			var editPage = providerWithoutLogging.GetRequiredService<EditPageViewModel>();

			Assert.IsNull(editPage.Model, "Expected the parameterless-constructor fallback to leave Model unset.");
			Assert.IsNull(editPage.EditService, "Expected the parameterless-constructor fallback to leave EditService unset.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_ComposedLikeApp_ResolvesEveryGraphNodeAndFacade()
		{
			var nodeTypes = new List<Type>
			{
				typeof(InputImageNode), typeof(CroppedImageNode), typeof(FilteredImageNode),
				typeof(EditedImageNode), typeof(DataPointsNode), typeof(DataNode),
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
		public void AddWpfServices_ComposedLikeApp_ResolvesEveryWpfServiceAndKeyedOcrService()
		{
			Assert.IsNotNull(provider.GetRequiredService<IWindowService>());
			Assert.IsNotNull(provider.GetRequiredService<IDownloadService>());
			Assert.IsNotNull(provider.GetRequiredService<IFileDialogService>());
			Assert.IsNotNull(provider.GetRequiredService<IMessageBoxService>());
			Assert.IsNotNull(provider.GetRequiredService<IAwaitTaskService>());
			Assert.IsNotNull(provider.GetRequiredService<IClipboardService>());
			Assert.IsNotNull(provider.GetRequiredService<IEditService<Image<Rgba, byte>>>());

			// App.xaml.cs registers two keyed OCR engines that RangePageViewModel depends on by key.
			Assert.IsNotNull(provider.GetRequiredKeyedService<IOcrService>("Numerical"));
			Assert.IsNotNull(provider.GetRequiredKeyedService<IOcrService>("Text"));
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_ResolvingIPageServiceTwice_ReturnsTheSameSingleton()
		{
			// App.xaml.cs registers IPageService as a singleton itself; AddModel()/AddViewModels()
			// don't grant this - it's a WPF-specific composition decision worth guarding.
			Assert.AreSame(provider.GetRequiredService<IPageService>(), provider.GetRequiredService<IPageService>());
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_ResolvingEditPageViewModelInDifferentScopes_ReturnsDifferentInstances()
		{
			using var scopeA = provider.CreateScope();
			using var scopeB = provider.CreateScope();

			var editPageA = scopeA.ServiceProvider.GetRequiredService<EditPageViewModel>();
			var editPageB = scopeB.ServiceProvider.GetRequiredService<EditPageViewModel>();

			Assert.AreNotSame(editPageA, editPageB, "EditPageViewModel must be scoped so undo history doesn't leak between scopes.");
			Assert.AreNotSame(editPageA.EditService, editPageB.EditService, "The scoped IEditService<T> must not be shared across scopes either.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_ResolvingEditPageViewModelWithinTheSameScope_ReturnsTheSameInstance()
		{
			using var scope = provider.CreateScope();

			var first = scope.ServiceProvider.GetRequiredService<EditPageViewModel>();
			var second = scope.ServiceProvider.GetRequiredService<EditPageViewModel>();

			Assert.AreSame(first, second, "A scoped service must resolve to the same instance within one scope.");
		}

		[TestMethod]
		[TestCategory("Integration")]
		public void AddWpfServices_ResolvingScopedEditServiceFromRootProviderDirectly_ReturnsTheSameInstanceAcrossCalls()
		{
			// BuildServiceProvider() defaults to ValidateScopes: false, so resolving a scoped
			// service straight from the root provider does not throw. Instead, the root provider
			// acts as its own ambient scope: repeated resolutions return the same cached instance,
			// the same way two resolves in a single explicit scope would. This documents that
			// current behaviour so a future change to ServiceProviderOptions is a deliberate one.
			var first = provider.GetRequiredService<IEditService<Image<Rgba, byte>>>();
			var second = provider.GetRequiredService<IEditService<Image<Rgba, byte>>>();

			Assert.AreSame(first, second);
		}
	}
}
