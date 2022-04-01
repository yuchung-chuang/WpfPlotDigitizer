using Emgu.CV;
using Emgu.CV.Structure;

using GalaSoft.MvvmLight.Command;

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

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for PreviewPage.xaml
	/// </summary>
	public partial class PreviewPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;
		private readonly IServiceProvider provider;
		private IEnumerable<PointD> data;
		private DataType dataType;

		public event PropertyChangedEventHandler PropertyChanged;

		public Image<Rgba, byte> Image { get; private set; }

		private IEnumerable<PointD> points;

		public Image<Rgba, byte> EdittedImage { get; private set; }
		public DataType DataType
		{
			get => dataType; 
			private set
			{
				if (value != dataType) {
					dataType = value;
					ExtractPoints();
				}
			}
		}

		public ImageSource ImageSource => Image?.ToBitmapSource();

		public RelayCommand ExportCommand { get; private set; }

		public bool IsDiscrete
		{
			get => DataType == DataType.Discrete;
			set => DataType = value ? DataType.Discrete : DataType.Continuous;
		}

		public bool IsContinuous
		{
			get => DataType == DataType.Continuous;
			set => DataType = value ? DataType.Continuous : DataType.Discrete;
		}

		public PreviewPage(Model model, IServiceProvider provider) : this()
		{
			this.model = model;
			this.provider = provider;
		}

		private PreviewPage()
		{
			InitializeComponent();
			ExportCommand = new RelayCommand(Export, CanExport);
			Loaded += PreviewPage_Loaded;
			Unloaded += PreviewPage_Unloaded;
		}
		private void ExtractPoints()
		{
			Image = EdittedImage.Copy();
			points = DataType switch
			{
				DataType.Discrete => Methods.GetDiscretePoints(Image),
				DataType.Continuous => Methods.GetContinuousPoints(Image),
				_ => throw new NotImplementedException(),
			};
			OnPropertyChanged(nameof(ImageSource));
			ExportCommand.RaiseCanExecuteChanged();
		}

		private void PreviewPage_Loaded(object sender, RoutedEventArgs e)
		{
			EdittedImage = model.EdittedImage.Copy();
			DataType = model.DataType;
			ExtractPoints();
		}

		private void PreviewPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.DataType = DataType;
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Export()
		{
			data = Methods.TransformData(points, new System.Drawing.Size(Image.Width, Image.Height), model.AxisLimit, model.AxisLogBase);
			var saveFileDialog = new SaveFileDialog
			{
				Filter =
				"CSV |*.csv|" +
				"TXT |*.txt"
			};
			if (saveFileDialog.ShowDialog() == false)
				return;

			TrySave(saveFileDialog.FilterIndex);

			ExportResults SaveAsCSV(CancellationToken token) => SaveText(",", token);
			ExportResults SaveAsTXT(CancellationToken token) => SaveText("\t", token);
			ExportResults SaveText(string seperator, CancellationToken token)
			{
				try {
					var strPath = saveFileDialog.FileName;

					var content = new StringBuilder();
					content.AppendLine("X" + seperator + "Y");
					foreach (var point in data) {
						content.AppendLine(point.X.ToString() + seperator + point.Y.ToString());
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
				catch (Exception) {
					return ExportResults.Failed;
				}
			}

			async void TrySave(int index)
			{
				Mouse.OverrideCursor = Cursors.Wait;

				var cts = new CancellationTokenSource();
				var token = cts.Token;
				var saveTask = Task.Run(() =>
				{
					return index switch
					{
						2 => SaveAsTXT(token),
						_ => SaveAsCSV(token),
					};
				}, token);
				var popup = provider.GetService<ProgressPopup>();
				popup.Canceled += (sender, e) =>
				{
					cts.Cancel();
				};
				popup.Show();
				var result = await saveTask;
#if DEBUG
				await Task.Delay(1000); 
#endif
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
					case ExportResults.None:
					default:
						break;
				}
			}
		}

		private bool CanExport() => data != null;
	}

	public enum ExportResults
	{
		None,
		Sucessful,
		Failed,
		Canceled
	}
}