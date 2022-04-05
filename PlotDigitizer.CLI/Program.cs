﻿using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.CommandLine;
using System.Threading.Tasks;
using System.CommandLine.Parsing;

namespace PlotDigitizer.CLI
{
	class Program
	{
		// $ PlotDigitizer

		// -i		/image/path 										(required)
		// -s		/setting/file/path 									(required)
		// -o		/output/file/path									(required)

		// -h		help menu											(independent)
		// -v		about menu											(independent)
		static async Task Main(string[] args)
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

			await rootCommand.InvokeAsync(args);
		}

		private static void Run(FileInfo imageFile, FileInfo settingFile, FileInfo outputFile)
		{
			var model = new Model();

			var image = Image.FromFile(imageFile.FullName) as Bitmap;
			model.InputImage = image.ToImage<Rgba, byte>();

			var json = File.ReadAllText(settingFile.FullName);
			var setting = JsonSerializer.Deserialize(json, typeof(Setting)) as Setting;
			model.Load(setting);

			SaveData(model.Data, outputFile.FullName);
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
						throw new FormatException("Output file format is not recognized. Please use either .csv or .txt as file extension.");
				}
				Console.WriteLine("The data has been exported successfully.");
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine("Something went wrong... try again? (y/n)");
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