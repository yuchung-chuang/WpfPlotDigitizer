using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PlotDigitizer.Core;

using WebModel = PlotDigitizer.Web.Models.Model;

namespace PlotDigitizer.Web.Services
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers the digitization node graph with a <em>scoped</em> lifetime.
		///
		/// <see cref="ServiceExtensions.AddModel"/> registers everything as a singleton, which is
		/// right for a single user desktop app and wrong for a server: it would give every visitor
		/// the same image, axis, filter and dataset. Each <see cref="DigitizerState"/> creates its
		/// own scope, so a scoped registration yields one private graph per browser session.
		/// </summary>
		public static IServiceCollection AddSessionScopedModel(this IServiceCollection services)
		{
			return services
				.AddScoped<InputImageNode>()
				.AddScoped<CroppedImageNode>()
				.AddScoped<FilteredImageNode>()
				.AddScoped<EditedImageNode>()
				.AddScoped<DataPointsNode>()
				.AddScoped<DataNode>()

				.AddScoped<AxisLocationNode>()
				.AddScoped<AxisTextBoxNode>()
				.AddScoped<AxisLimitNode>()
				.AddScoped<AxisTitleNode>()
				.AddScoped<AxisLogBaseNode>()
				.AddScoped<FilterMinNode>()
				.AddScoped<FilterMaxNode>()
				.AddScoped<DataTypeNode>()

				.AddScoped<Setting, UpdatableSetting>()
				.AddScoped<WebModel>()
				.AddScoped<Model>(sp => sp.GetRequiredService<WebModel>())
				.AddScoped<IEditService<Image<Rgba, byte>>, EditService<Image<Rgba, byte>>>();
		}

		/// <summary>
		/// Registers the shared Core services the web frontend needs, including the two OCR engines
		/// configured the same way the desktop app configures them.
		/// </summary>
		public static IServiceCollection AddDigitizerServices(this IServiceCollection services, IConfiguration configuration)
		{
			return services
				.AddTransient<IImageService, EmguCvService>()
				.AddTransient<IDownloadService, DownloadService>()
				.AddScoped<IMessageBoxService, CollectingMessageBoxService>()
				.AddScoped(sp => (CollectingMessageBoxService)sp.GetRequiredService<IMessageBoxService>())

				// Tesseract engines are expensive to construct and are stateless between calls.
				.AddOcrServices(configuration);
		}
	}
}
