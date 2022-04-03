using Microsoft.Win32;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Serialization;

namespace PlotDigitizer.App
{
	[AddINotifyPropertyChangedInterface]
	public partial class MainWindow : Window
	{
		private readonly Model model;

		public PageManager PageManager { get; private set; }

		public IEnumerable<string> PageNameList { get; private set; }

		public RelayCommand SaveSettingCommand { get; set; }

		public RelayCommand LoadSettingCommand { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			SaveSettingCommand = new RelayCommand(SaveSetting);
			LoadSettingCommand = new RelayCommand(LoadSetting);
		}


		public MainWindow(PageManager pageManager, Model model) : this()
		{
			this.model = model;
			PageManager = pageManager;
			PageNameList = pageManager.PageList.Select(page => page.GetType().Name);
		}

		private void SaveSetting()
		{
			var dialog = new SaveFileDialog
			{
				Filter = "JSON file (*.json) |*.json|" +
				"XML file (*.xml) |*.xml",
				FileName = "PlotDigitizer Setting"
			};
			if (dialog.ShowDialog() != true) {
				return;
			}
			var index = dialog.FilterIndex;
			switch (index) {
				default:
				case 1:
					var json = JsonSerializer.Serialize(model.Setting, new JsonSerializerOptions()
					{
						Converters =
						{
							new RgbaConverter(),
						}
					});
					File.WriteAllText(dialog.FileName, json);
					break;
				case 2: {
						var xmlSerializer = new XmlSerializer(typeof(Setting));
						using var stream = new FileStream(dialog.FileName, FileMode.OpenOrCreate);
						xmlSerializer.Serialize(stream, model.Setting);
						break;
					}
			}
			MessageBox.Show("Current setting is saved successfully.", "PlotDigitizer", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void LoadSetting()
		{
			var dialog = new OpenFileDialog()
			{
				Filter = "JSON file (*.json) |*.json|" +
				"XML file (*.xml) |*.xml",
				FileName = "PlotDigitizer Setting"
			};
			if (dialog.ShowDialog() != true) {
				return;
			}
			var ext = Path.GetExtension(dialog.FileName).ToLower();
			Setting setting = null;
			switch (ext) {
				case ".json":
					var json = File.ReadAllText(dialog.FileName);
					setting = JsonSerializer.Deserialize(json, typeof(Setting), new JsonSerializerOptions()
					{
						Converters =
						{
							new RgbaConverter(),
						}
					}) as Setting;
					break;
				case ".xml": {
						var xmlSerializer = new XmlSerializer(typeof(Setting));
						using var stream = new FileStream(dialog.FileName, FileMode.Open);
						setting = xmlSerializer.Deserialize(stream) as Setting;
						break;
					}
				default:
					MessageBox.Show("Cannot load this file. File extension must be either .json or .xml", "PlotDigitizer", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
			}
			model.Load(setting);
			MessageBox.Show("Setting is loaded successfully.", "PlotDigitizer", MessageBoxButton.OK, MessageBoxImage.Information);
		}


		private void mainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			// disable frame navigation to prevent keyboard input conflict
			if (e.NavigationMode == NavigationMode.Back ||
				e.NavigationMode == NavigationMode.Forward) {
				e.Cancel = true;
			}
		}
	}
}
