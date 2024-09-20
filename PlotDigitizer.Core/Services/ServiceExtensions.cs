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
			return services.AddTransient<MainWindowViewModel>()
			.AddTransient<LoadPageViewModel>()
			.AddTransient<RangePageViewModel>()
			.AddTransient<AxisPageViewModel>()
			.AddTransient<FilterPageViewModel>()
			.AddTransient<EditPageViewModel>()
			.AddTransient<DataPageViewModel>();
		}

		public static IServiceCollection AddModel(this IServiceCollection services)
		{
			return services.AddSingleton<Model, ModelFacade>()
			.AddSingleton<Setting, SettingFacade>()
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
