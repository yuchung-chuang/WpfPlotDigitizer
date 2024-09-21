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
				.AddSingleton<EditPageViewModel>() // TODO: this should be scoped, it's life cycle is in between every change in filtered image.
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

				.AddSingleton<AxisLimitNode>()
				.AddSingleton<AxisLogBaseNode>()
				.AddSingleton<AxisLocationNode>()
				.AddSingleton<FilterMinNode>()
				.AddSingleton<FilterMaxNode>()
				.AddSingleton<DataTypeNode>();
		}
	}
}
