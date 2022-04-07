using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.App
{
	public class PreviewPageViewModel : ViewModelBase
	{
		private readonly IMessageBoxService messageBoxService;
		private readonly IFileDialogService fileDialogService;
		private readonly IAwaitTaskService awaitTaskService;

		public RelayCommand ExportCommand { get; private set; }

		[OnChangedMethod(nameof(OnIsDiscreteChanged))]
		public bool IsDiscrete
		{
			get => Model?.Setting.DataType == DataType.Discrete;
			set => Model.Setting.DataType = value ? DataType.Discrete : DataType.Continuous;
		}

		[OnChangedMethod(nameof(OnIsContinuousChanged))]
		public bool IsContinuous
		{
			get => Model?.Setting.DataType == DataType.Continuous;
			set => Model.Setting.DataType = value ? DataType.Continuous : DataType.Discrete;
		}
		public bool IsEnabled => Model != null && Model.EdittedImage != null;

		public Model Model { get; }
		public PreviewPageViewModel()
		{
			ExportCommand = new RelayCommand(Export, CanExport);
		}
		public PreviewPageViewModel(Model model, 
			IMessageBoxService messageBoxService, 
			IFileDialogService fileDialogService,
			IAwaitTaskService awaitTaskService) : this()
		{
			Model = model;
			this.messageBoxService = messageBoxService;
			this.fileDialogService = fileDialogService;
			this.awaitTaskService = awaitTaskService;
			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;

		}
		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Model.EdittedImage)) {
				OnPropertyChanged(nameof(IsEnabled));
				ExtractPoints();
			}
		}

		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is Setting setting)) {
				return;
			}
			if (e.PropertyName == nameof(setting.DataType)) {
				OnPropertyChanged(nameof(IsDiscrete));
				OnPropertyChanged(nameof(IsContinuous));
			}
		}

		private void OnIsDiscreteChanged() => ExtractPoints();
		private void OnIsContinuousChanged() => ExtractPoints();

		public void ExtractPoints()
		{
			if (!IsEnabled) {
				return;
			}
			Model.ExtractData();
		}

		private void Export()
		{
			var result = fileDialogService.SaveFileDialog();
			if (!result.IsValid)
				return;

			TrySave(result.FileName);

			async void TrySave(string fileName)
			{
				var result = await awaitTaskService.RunAsync((token) =>
				{
					try {
						return Path.GetExtension(fileName).ToLower() switch
						{
							".txt" => SaveAsTXT(token),
							".csv" => SaveAsCSV(token),
							_ => throw new FormatException("Output file format is not recognized. Please use either .csv or .txt as file extension."),
						};
					}
					catch (Exception) {
						return ExportResults.Failed;
					}
				});


				switch (result) {
					case ExportResults.Sucessful:
						messageBoxService.Show_OK("The data has been exported successfully.", "Notification");
						break;
					case ExportResults.Failed: {
							var response = messageBoxService.Show_Warning_OkCancel("Something went wrong... try again?", "Error");
							if (response) {
								TrySave(fileName);
							}

							break;
						}
					case ExportResults.Canceled:
						messageBoxService.Show_OK("Export operation has been cancelled.", "Notification");
						break;
					case ExportResults.None:
					default:
						break;
				}

				ExportResults SaveAsCSV(CancellationToken token) => SaveText(",", token);
				ExportResults SaveAsTXT(CancellationToken token) => SaveText("\t", token);
				ExportResults SaveText(string seperator, CancellationToken token)
				{
					var content = new StringBuilder();
					content.AppendLine("X" + seperator + "Y");
					foreach (var point in Model.Data) {
						content.AppendLine(point.X.ToString() + seperator + point.Y.ToString());
#if DEBUG
						Thread.Sleep(250);
						//throw new Exception("Test error");
#endif
						if (token.IsCancellationRequested) {
							return ExportResults.Canceled;
						}
					}

					using (var fs = File.OpenWrite(fileName))
					using (var sw = new StreamWriter(fs)) {
						sw.Write(content.ToString());
					}

					return ExportResults.Sucessful;
				}
			}
		}

		private bool CanExport() => Model?.Data?.Count() > 0;
	}
}
