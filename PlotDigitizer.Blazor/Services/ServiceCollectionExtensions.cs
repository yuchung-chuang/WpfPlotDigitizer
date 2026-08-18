using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;

using PlotDigitizer.Core;

using BlazorModel = PlotDigitizer.Blazor.Models.Model;

namespace PlotDigitizer.Blazor.Services
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Registers the digitization node graph with a <em>scoped</em> lifetime.
		///
		/// <see cref="ServiceExtensions.AddModel"/> registers everything as a singleton, which is
		/// right for a single user desktop app and wrong for a server: it would give every visitor
		/// the same image, axis, and filter. Each <see cref="DigitizerState"/> creates its own
		/// scope, so a scoped registration yields one private graph per browser session.
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
				.AddScoped<BlazorModel>()
				.AddScoped<Model>(sp => sp.GetRequiredService<BlazorModel>());
		}

		/// <summary>
		/// Registers the shared Core services this spike needs. Unlike
		/// <c>PlotDigitizer.Web.Services.ServiceCollectionExtensions.AddDigitizerServices</c>, OCR
		/// is left out entirely: Load and Filter never touch axis detection, so pulling in
		/// Tesseract and its training data would be pure scope creep for this spike.
		/// </summary>
		public static IServiceCollection AddDigitizerServices(this IServiceCollection services)
		{
			return services.AddTransient<IImageService, EmguCvService>();
		}
	}
}
