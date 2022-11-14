using PropertyChanged;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using System.Xml.Serialization;

namespace PlotDigitizer.Core
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields

		private readonly IFileDialogService fileDialogService;
		private readonly IMessageBoxService messageBoxService;
		private readonly Setting setting;
		private int currentTabIndex = 0;

		#endregion Fields

		#region Properties
		public int CurrentTabIndex
		{
			get => currentTabIndex;
			set
			{
				if (currentTabIndex == value)
					return;
				CurrentTab.Leave();
				currentTabIndex = value;
				PrevPageCommand.RaiseCanExecuteChanged();
				NextPageCommand.RaiseCanExecuteChanged();
				OnPropertyChanged(nameof(CurrentTab));
				CurrentTab.Enter();
			}
		}

		public PageViewModelBase CurrentTab { get; set; }
		public RelayCommand NextPageCommand { get; }
		public RelayCommand PrevPageCommand { get; }

		public RelayCommand LoadSettingCommand { get; set; }
		public RelayCommand SaveSettingCommand { get; set; }
		public Model Model { get; }

		#endregion Properties

		#region Constructors

		public MainWindowViewModel()
		{
			SaveSettingCommand = new RelayCommand(SaveSetting);
			LoadSettingCommand = new RelayCommand(LoadSetting);
			NextPageCommand = new RelayCommand(NextPage, CanNextPage);
			PrevPageCommand = new RelayCommand(PrevPage, CanPrevPage);
		}

		private void PrevPage() => CurrentTabIndex--;
		private bool CanPrevPage() => CurrentTabIndex > 0;
		private void NextPage() => CurrentTabIndex++;
		private bool CanNextPage() => CurrentTabIndex < 6 - 1;

		public MainWindowViewModel(
			Model model,
			Setting setting,
			IFileDialogService fileDialogService,
			IMessageBoxService messageBoxService) : this()
		{
			Model = model;
			this.setting = setting;
			this.fileDialogService = fileDialogService;
			this.messageBoxService = messageBoxService;
		}

		#endregion Constructors

		#region Methods

		private void LoadSetting()
		{
			var filter = "JSON file (*.json) |*.json|" +
				"XML file (*.xml) |*.xml";
			var filename = "PlotDigitizer Setting";
			var results = fileDialogService.OpenFileDialog(filter, filename);
			if (!results.IsValid) {
				return;
			}
			var ext = Path.GetExtension(results.FileName).ToLower();
			Setting setting = null;
			switch (ext) {
				case ".json":
					var json = File.ReadAllText(results.FileName);
					setting = JsonSerializer.Deserialize(json, typeof(Setting), new JsonSerializerOptions
					{
						Converters = { new RgbaConverter() }
					}) as Setting;
					break;

				case ".xml": {
						var xmlSerializer = new XmlSerializer(typeof(Setting));
						using var stream = new FileStream(results.FileName, FileMode.Open);
						setting = xmlSerializer.Deserialize(stream) as Setting;
						break;
					}
				default:
					messageBoxService.Show_OK("Cannot load this file. File extension must be either .json or .xml", "PlotDigitizer");
					return;
			}
			this.setting.Load(setting);
			messageBoxService.Show_OK("Setting is loaded successfully.", "PlotDigitizer");
		}

		private void SaveSetting()
		{
			var filter = "JSON file (*.json) |*.json|" +
				"XML file (*.xml) |*.xml";
			var filename = "PlotDigitizer Setting";
			var results = fileDialogService.SaveFileDialog(filter, filename);
			if (!results.IsValid) {
				return;
			}
			switch (Path.GetExtension(results.FileName).ToLower()) {
				default:
				case ".json":
					var json = JsonSerializer.Serialize(setting, new JsonSerializerOptions
					{
						Converters = { new RgbaConverter() }
					});
					File.WriteAllText(results.FileName, json);
					break;

				case ".xml": {
						var xmlSerializer = new XmlSerializer(typeof(Setting));
						using var stream = new FileStream(results.FileName, FileMode.OpenOrCreate);
						xmlSerializer.Serialize(stream, setting.Copy());
						break;
					}
			}
			messageBoxService.Show_OK("Current setting is saved successfully.", "PlotDigitizer");
		}

		#endregion Methods
	}
}