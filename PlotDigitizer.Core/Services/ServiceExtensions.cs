using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.IO;

namespace PlotDigitizer.Core
{
    public static class ServiceExtensions
	{
		public static IServiceCollection AddViewModels(this IServiceCollection services)
		{
			return services
				.AddTransient<MainViewModel>()
				.AddTransient<LoadPageViewModel>()
				.AddTransient<RangePageViewModel>()
				.AddTransient<AxisPageViewModel>()
				.AddTransient<FilterPageViewModel>()
				.AddScoped<IEditService<Image<Rgba, byte>>, EditService<Image<Rgba, byte>>>()
				.AddScoped<EditPageViewModel>()
				.AddTransient<DataPageViewModel>();
		}

		public static IServiceCollection AddModel(this IServiceCollection services)
		{
			return services
				.AddSingleton<Model, UpdatableModel>()
				.AddSingleton<Setting, UpdatableSetting>()
				.AddSingleton<InputImageNode>()
				.AddSingleton<CroppedImageNode>()
				.AddSingleton<FilteredImageNode>()
				.AddSingleton<EditedImageNode>()
				.AddSingleton<DataPointsNode>()
				.AddSingleton<DataNode>()

				.AddSingleton<AxisLocationNode>()
				.AddSingleton<AxisTextBoxNode>()
				.AddSingleton<AxisLimitNode>()
				.AddSingleton<AxisTitleNode>()
				.AddSingleton<AxisLogBaseNode>()
				.AddSingleton<FilterMinNode>()
				.AddSingleton<FilterMaxNode>()
				.AddSingleton<DataTypeNode>();
		}

		/// <summary>
		/// Registers the two keyed OCR engines ("Numerical" and "Text") that Core's
		/// <see cref="IOcrService"/> consumers expect, binding each from the configuration's
		/// <c>OCR:Numerical</c>/<c>OCR:Text</c> sections. Shared by every frontend that offers OCR
		/// axis detection (WPF and Web) so the two engines are always configured the same way.
		/// </summary>
		/// <param name="resolveDataPath">
		/// Anchors a configured <see cref="OcrSettings.DataPath"/> to an absolute path. Defaults to
		/// resolving relative to <see cref="AppContext.BaseDirectory"/>, which is safe regardless of
		/// the process's current working directory. Pass a different resolver only if a frontend has
		/// a different notion of "next to the assembly".
		/// </param>
		public static IServiceCollection AddOcrServices(this IServiceCollection services, IConfiguration configuration, Func<string, string> resolveDataPath = null)
		{
			resolveDataPath ??= ResolveDataPath;

			return services
				.AddKeyedSingleton<IOcrService, OcrService>("Numerical")
				.Configure<OcrSettings>("Numerical", settings => Bind(configuration, "Numerical", settings, resolveDataPath))
				.AddKeyedSingleton<IOcrService, OcrService>("Text")
				.Configure<OcrSettings>("Text", settings => Bind(configuration, "Text", settings, resolveDataPath));

			static void Bind(IConfiguration configuration, string name, OcrSettings settings, Func<string, string> resolveDataPath)
			{
				var section = configuration.GetSection("OCR").GetSection(name);
				settings.DataPath = resolveDataPath(section[nameof(settings.DataPath)]);
				settings.Language = section[nameof(settings.Language)];
				settings.WhiteList = section[nameof(settings.WhiteList)];
			}
		}

		// The training data is copied next to the assembly, but a frontend's working directory is
		// not guaranteed to be the output folder (e.g. a web host, or a desktop shortcut with a
		// different "start in" folder), so a relative path from configuration has to be anchored.
		private static string ResolveDataPath(string configured)
		{
			var path = string.IsNullOrWhiteSpace(configured) ? "OCR" : configured;
			if (!Path.IsPathRooted(path)) {
				path = Path.Combine(AppContext.BaseDirectory, path);
			}
			return path + Path.DirectorySeparatorChar;
		}
	}
}
