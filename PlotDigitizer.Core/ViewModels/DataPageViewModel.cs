using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;

using PropertyChanged;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PlotDigitizer.Core
{
    public enum ExportResults
	{
		None,
		Canceled,
		Failed,
		Sucessful,
	}

	public class DataPageViewModel : ViewModelBase
	{
		#region Fields

		private readonly IAwaitTaskService awaitTask;
		private readonly IFileDialogService fileDialog;
		private readonly ILogger<DataPageViewModel> logger;
		private readonly IMessageBoxService messageBox;
		private readonly Setting setting;

		#endregion Fields

		#region Properties

		public RelayCommand ExportCommand { get; private set; }

		[OnChangedMethod(nameof(OnIsContinuousChanged))]
		public bool IsContinuous
		{
			get => setting.DataType == DataType.Continuous;
			set => setting.DataType = value ? DataType.Continuous : DataType.Discrete;
		}

		[OnChangedMethod(nameof(OnIsDiscreteChanged))]
		public bool IsDiscrete
		{
			get => setting.DataType == DataType.Discrete;
			set => setting.DataType = value ? DataType.Discrete : DataType.Continuous;
		}

		public bool IsEnabled => Model != null && Model.EdittedImage != null;

		public Model Model { get; }

		public Image<Rgba, byte> Image => !IsEnabled ? null : Model.PreviewImage;

		#endregion Properties

		#region Constructors

		public DataPageViewModel()
		{
			Name = "Data Page";
			ExportCommand = new RelayCommand(Export, CanExport);
		}

		public DataPageViewModel(
			Model model,
			Setting setting,
			IMessageBoxService messageBox,
			IFileDialogService fileDialog,
			IAwaitTaskService awaitTask,
			ILogger<DataPageViewModel> logger) : this()
		{
			Model = model;
			this.setting = setting;
			this.messageBox = messageBox;
			this.fileDialog = fileDialog;
			this.awaitTask = awaitTask;
			this.logger = logger;
		}

		#endregion Constructors

		#region Methods

		public override void Enter()
		{
			base.Enter();
			if (!IsEnabled) {
				return;
			}
			ExtractPoints();
		}

		public void ExtractPoints()
		{
			if (!IsEnabled) {
				return;
			}
			Model.RaisePropertyChanged(nameof(Core.Model.PreviewImage));
			ExportCommand.RaiseCanExecuteChanged();
			logger.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		public override void Leave() => base.Leave();

		private bool CanExport() => Model?.Data?.Count() > 0;

		private void Export()
		{
			var filter = "Comma-separated values file (*.csv) |*.csv|" +
				"Text file (*.txt) |*.txt|" +
				"Any (*.*) |*.*";
			var result = fileDialog.SaveFileDialog(filter, "output");
			if (!result.IsValid)
				return;

			TrySave(result.FileName);

			async void TrySave(string fileName)
			{
				var result = await awaitTask.RunAsync((token) =>
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
						messageBox.Show_OK("The data has been exported successfully.", "Notification");
						break;

					case ExportResults.Failed: {
							var response = messageBox.Show_Warning_OkCancel("Something went wrong... try again?", "Error");
							if (response) {
								TrySave(fileName);
							}

							break;
						}
					case ExportResults.Canceled:
						messageBox.Show_OK("Export operation has been cancelled.", "Notification");
						break;

					case ExportResults.None:
					default:
						break;
				}
				logger.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");

				ExportResults SaveAsCSV(CancellationToken token) => SaveText(",", token);
				ExportResults SaveAsTXT(CancellationToken token) => SaveText("\t", token);
				ExportResults SaveText(string seperator, CancellationToken token)
				{
					var content = new StringBuilder();
					var xlabel = !string.IsNullOrWhiteSpace(setting.AxisTitle.XLabel) ?
						setting.AxisTitle.XLabel : "X";
					var ylabel = !string.IsNullOrWhiteSpace(setting.AxisTitle.YLabel) ? setting.AxisTitle.YLabel : "Y";
                    content.AppendLine(xlabel + seperator + ylabel);
					foreach (var point in Model.Data) {
						content.AppendLine(point.X.ToString() + seperator + point.Y.ToString());
						if (token.IsCancellationRequested) {
							return ExportResults.Canceled;
						}
					}

					using (var fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
					using (var sw = new StreamWriter(fs)) {
						sw.Write(content.ToString());
					}

					return ExportResults.Sucessful;
				}
			}
		}

		private void OnIsContinuousChanged()
		{
			ExtractPoints();
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		private void OnIsDiscreteChanged()
		{
			ExtractPoints();
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		#endregion Methods
	}
}