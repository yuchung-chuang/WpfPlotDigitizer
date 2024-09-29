using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
				.AddSingleton<EdittedImageNode>()
				.AddSingleton<PreviewImageNode>()
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
	}
}
