using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Logging;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace PlotDigitizer.Core
{
	public class Model : INotifyPropertyChanged
	{
		private readonly ILogger<Model> logger;

		[OnChangedMethod(nameof(OnInputImageChanged))]
		public Image<Rgba, byte> InputImage { get; set; }

		[OnChangedMethod(nameof(OnCroppedImageChanged))]
		public Image<Rgba, byte> CroppedImage { get; private set; }

		[OnChangedMethod(nameof(OnFilteredImageChanged))]
		public Image<Rgba, byte> FilteredImage { get; private set; }

		[OnChangedMethod(nameof(OnEdittedImageChanged))]
		public Image<Rgba, byte> EdittedImage { get; set; }

		public Image<Rgba, byte> PreviewImage { get; private set; }

		public IEnumerable<PointD> Data { get; private set; }

		public Setting Setting { get; private set; } = new Setting();

		public event PropertyChangedEventHandler PropertyChanged;

		public Model()
		{
			Setting.PropertyChanged += Setting_PropertyChanged;
		}

		public Model(ILogger<Model> logger) : this()
		{
			this.logger = logger;
		}

		public void Load(Setting setting)
		{
			foreach (var property in typeof(Setting).GetProperties()) {
				var value = property.GetValue(setting);
				if (value != default) {
					property.SetValue(Setting, value);
				}
			}
			logger.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		public void ExtractData()
		{
			PreviewImage = EdittedImage.Copy();
			var points = Setting.DataType switch
			{
				DataType.Discrete => Methods.GetDiscretePoints(PreviewImage),
				DataType.Continuous => Methods.GetContinuousPoints(PreviewImage),
				_ => throw new NotImplementedException(),
			};
			OnPropertyChanged(nameof(PreviewImage));
			Data = Methods.TransformData(points, new Size(PreviewImage.Width, PreviewImage.Height), Setting.AxisLimit, Setting.AxisLogBase);
			logger.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InputImage is null) {
				return;
			}
			switch (e.PropertyName) {
				case nameof(Setting.AxisLocation):
					CropImage();
					break;
				case nameof(Setting.FilterMin):
				case nameof(Setting.FilterMax):
					FilterImage();
					break;
				case nameof(Setting.AxisLogBase):
				case nameof(Setting.AxisLimit):
				case nameof(Setting.DataType):
					ExtractData();
					break;
				default:
					return;
			}
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}
		private void OnInputImageChanged()
		{
			if (Setting.AxisLocation == default) {
				Setting.AxisLocation = Methods.GetAxisLocation(InputImage) ?? new Rectangle(InputImage.Width / 4, InputImage.Height / 4, InputImage.Width / 2, InputImage.Height / 2);
				// image will be cropped once the AxisLocation is set
			} else {
				CropImage();
			}
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}
		private void OnCroppedImageChanged()
		{
			FilterImage();
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		private void OnFilteredImageChanged()
		{
			EdittedImage = FilteredImage;
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		private void OnEdittedImageChanged()
		{
			ExtractData();
			logger.LogDebug($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		private void CropImage()
		{
			CroppedImage = Methods.CropImage(InputImage, Setting.AxisLocation);
			logger.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}

		private void FilterImage()
		{
			FilteredImage = Methods.FilterRGB(CroppedImage, Setting.FilterMin, Setting.FilterMax);
			logger.LogInformation($"{GetType()}.{MethodBase.GetCurrentMethod().Name} completed.");
		}
	}

	public enum SaveType
	{
		CSV,
		TXT,
	}
}