using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Win32;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using PropertyChanged;

namespace PlotDigitizer.App
{
	public partial class PreviewPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;
		private readonly IServiceProvider provider;

		public event PropertyChangedEventHandler PropertyChanged;

		public ImageSource ImageSource => model?.PreviewImage?.ToBitmapSource();

		public RelayCommand ExportCommand { get; private set; }

		[OnChangedMethod(nameof(OnIsDiscreteChanged))]
		public bool IsDiscrete
		{
			get => model?.Setting.DataType == DataType.Discrete;
			set => model.Setting.DataType = value ? DataType.Discrete : DataType.Continuous;
		}

		[OnChangedMethod(nameof(OnIsContinuousChanged))]
		public bool IsContinuous
		{
			get => model?.Setting.DataType == DataType.Continuous;
			set => model.Setting.DataType = value ? DataType.Continuous : DataType.Discrete;
		}
		public bool Enabled => model != null && model.EdittedImage != null;

		private PreviewPage()
		{
			InitializeComponent();
			ExportCommand = new RelayCommand(Export, CanExport);
			Loaded += PreviewPage_Loaded;
		}
		public PreviewPage(Model model, IServiceProvider provider) : this()
		{
			this.model = model;
			this.provider = provider;
			model.PropertyChanged += Model_PropertyChanged;
			model.Setting.PropertyChanged += Setting_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.EdittedImage)) {
				OnPropertyChanged(nameof(Enabled));
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

		private void PreviewPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (!Enabled) {
				return;
			}
			ExtractPoints();
		}

		private void ExtractPoints()
		{
			if (!Enabled) {
				return;
			}
			model.ExtractData();
			OnPropertyChanged(nameof(ImageSource));
		}
		private void OnIsDiscreteChanged() => ExtractPoints();
		private void OnIsContinuousChanged() => ExtractPoints();

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Export()
		{
			var saveFileDialog = new SaveFileDialog
			{
				Filter =
				"Comma-separated values file (*.csv) |*.csv|" +
				"Text file (*.txt) |*.txt|" +
				"Any (*.*) |*.*",
				FileName = "output",
			};
			if (saveFileDialog.ShowDialog() != true)
				return;

			TrySave(saveFileDialog.FilterIndex);

			ExportResults SaveAsCSV(CancellationToken token) => SaveText(",", token);
			ExportResults SaveAsTXT(CancellationToken token) => SaveText("\t", token);
			ExportResults SaveText(string seperator, CancellationToken token)
			{
				var strPath = saveFileDialog.FileName;

				var content = new StringBuilder();
				content.AppendLine("X" + seperator + "Y");
				foreach (var point in model.Data) {
					content.AppendLine(point.X.ToString() + seperator + point.Y.ToString());
#if DEBUG
					Thread.Sleep(250);
					//throw new Exception("Test error");
#endif
					if (token.IsCancellationRequested) {
						return ExportResults.Canceled;
					}
				}

				using (var fs = File.OpenWrite(strPath))
				using (var sw = new StreamWriter(fs)) {
					sw.Write(content.ToString());
				}

				return ExportResults.Sucessful;
			}

			async void TrySave(int index)
			{
				Mouse.OverrideCursor = Cursors.Wait;

				var cts = new CancellationTokenSource();
				var token = cts.Token;

				var saveTask = new Task<ExportResults>(() =>
				{
					try {
						return index switch
						{
							2 => SaveAsTXT(token),
							_ => SaveAsCSV(token),
						};
					}
					catch (Exception) {
						return ExportResults.Failed;
					}
				}, token);
				var popup = provider.GetService<ProgressPopup>();
				popup.Canceled += (sender, e) =>
				{
					cts.Cancel();
				};
				popup.Show();

				saveTask.Start();
				var result = await saveTask;
				popup.Close();
				Mouse.OverrideCursor = null;

				switch (result) {
					case ExportResults.Sucessful:
						MessageBox.Show("The data has been exported successfully.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
						break;
					case ExportResults.Failed: {
							var response = MessageBox.Show("Something went wrong... try again?", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel);
							if (response == MessageBoxResult.OK) {
								TrySave(index);
							}

							break;
						}
					case ExportResults.Canceled:
						MessageBox.Show("Export operation has been cancelled.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
						break;
					case ExportResults.None:
					default:
						break;
				}
			}
		}

		private bool CanExport() => model?.Data?.Count() > 0;
	}

	public enum ExportResults
	{
		None,
		Sucessful,
		Failed,
		Canceled
	}
}