﻿using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using PlotDigitizer.Core;

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlotDigitizer.CLI
{
	internal class Program
	{
		private static ServiceProvider serviceProvider;
		private static ILogger<Program> logger;

		// $ PlotDigitizer

		// -i		/image/path 										(required)
		// -s		/setting/file/path 									(required)
		// -o		/output/file/path									(required)

		// -h		help menu											(independent)
		// -v		about menu											(independent)
		private static async Task Main(string[] args)
		{
			var rootCommand = ConfigureCommand();

			var services = new ServiceCollection()
					.AddSingleton<Model, UpdatableModel>()
					.AddSingleton<Setting, UpdatableSetting>()
					.AddViewModels()
					.AddModel()
				
				.AddLogging(builder =>
				{
					builder.ClearProviders() // to override the default set of logging providers added by the default host
						.AddProvider(new DebugLoggerProvider())
						.AddProvider(new FileLoggerProvider(Path.Combine(Directory.GetCurrentDirectory(), "logs")));
				});
			serviceProvider = services.BuildServiceProvider();

			
			logger = serviceProvider.GetRequiredService<ILogger<Program>>();
			logger.LogInformation("Application started.");

			logger.LogInformation("Command Invoking...");
			await rootCommand.InvokeAsync(args);
			logger.LogInformation("Command executed.");

			logger.LogInformation("Application terminated.");
		}

		private static RootCommand ConfigureCommand()
		{
			var imageOption = new Option<FileInfo>("-i").ExistingOnly();
			imageOption.AddAlias("--image");
			imageOption.Arity = ArgumentArity.ExactlyOne;
			imageOption.IsRequired = true;
			imageOption.Description = "The path of the input image.";

			var settingOption = new Option<FileInfo>("-s").ExistingOnly();
			settingOption.AddAlias("--setting");
			settingOption.Arity = ArgumentArity.ExactlyOne;
			settingOption.IsRequired = true;
			settingOption.Description = "The path of the setting file.";

			var outputOption = new Option<FileInfo>("-o");
			outputOption.AddAlias("--output");
			outputOption.Arity = ArgumentArity.ExactlyOne;
			outputOption.IsRequired = true;
			outputOption.Description = "The path of the output file.";

			var rootCommand = new RootCommand("Digitize data in an image.");
			rootCommand.AddOption(imageOption);
			rootCommand.AddOption(settingOption);
			rootCommand.AddOption(outputOption);

			rootCommand.SetHandler((FileInfo imageFile, FileInfo settingFile, FileInfo outputFile) =>
			{
				try {
					Run(imageFile, settingFile, outputFile);
				}
				catch (Exception ex) {
					Console.WriteLine(ex.Message);
				}
			}, imageOption, settingOption, outputOption);
			return rootCommand;
		}

		private static void Run(FileInfo imageFile, FileInfo settingFile, FileInfo outputFile)
		{
			var model = serviceProvider.GetRequiredService<Model>();
			var setting = serviceProvider.GetRequiredService<Setting>();

			logger.LogInformation("Loading image...");
			var image = Image.FromFile(imageFile.FullName) as Bitmap;
			model.InputImage = image.ToImage<Rgba, byte>();
			logger.LogInformation("Image loaded.");

			logger.LogInformation("Loading setting...");
			var json = File.ReadAllText(settingFile.FullName);
			var settingTmp = JsonSerializer.Deserialize(json, typeof(Setting)) as Setting;
			setting.Load(settingTmp);
			logger.LogInformation("Setting loaded.");

			logger.LogInformation("Saving data...");
			SaveData(model.Data, outputFile.FullName);
			logger.LogInformation("Data saved.");
		}

		private static void SaveData(IEnumerable<PointD> data, string fileName)
		{
			try {
				switch (Path.GetExtension(fileName).ToLower()) {
					case ".csv":
						SaveAsCSV();
						break;

					case ".txt":
						SaveAsTXT();
						break;

					default:
						var ex = new FormatException("Output file format is not recognized. Please use either .csv or .txt as file extension.");
						logger.LogCritical(ex.Message);
						throw ex;
				}
				logger.LogInformation("The data has been exported successfully.");
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				logger.LogInformation("Something went wrong... try again? (y/n)");
				var response = Console.ReadKey();
				Console.WriteLine(); // insert a line break after the response key
				if (response.Key == ConsoleKey.Y) {
					SaveData(data, fileName);
				}
			}
			void SaveAsCSV() => SaveText(",");
			void SaveAsTXT() => SaveText("\t");
			void SaveText(string seperator)
			{
				var content = new StringBuilder();
				content.AppendLine("X" + seperator + "Y");
				foreach (var point in data) {
					content.AppendLine(point.X.ToString() + seperator + point.Y.ToString());
				}

				using var fs = File.OpenWrite(fileName);
				using var sw = new StreamWriter(fs);
				sw.Write(content.ToString());
			}
		}
	}
}