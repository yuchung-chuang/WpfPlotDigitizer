using Emgu.CV;
using Emgu.CV.Structure;

using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class UpdatableModel : Model
	{
		private readonly InputImageNode inputImage;
		private readonly CroppedImageNode croppedImage;
		private readonly FilteredImageNode filteredImage;
		private readonly EdittedImageNode edittedImage;
		private readonly PreviewImageNode previewImage;
		private readonly DataNode data;

		public override Image<Rgba, byte> InputImage
		{
			get => inputImage.Get();
			set => inputImage.Set(value);
		}

		public override Image<Rgba, byte> CroppedImage
		{
			get => croppedImage.Get();
			set => croppedImage.Set(value);
		}

		public override Image<Rgba, byte> FilteredImage
		{
			get => filteredImage.Get();
			set => filteredImage.Set(value);
		}

		public override Image<Rgba, byte> EdittedImage
		{
			get => edittedImage.Get();
			set => edittedImage.Set(value);
		}

		public override Image<Rgba, byte> PreviewImage
		{
			get => previewImage.Get();
			set => previewImage.Set(value);
		}

		public override IEnumerable<PointD> Data
		{
			get => data.Get();
			set => data.Set(value);
		}

		public UpdatableModel(InputImageNode inputImage,
			CroppedImageNode croppedImage,
			FilteredImageNode filteredImage,
			EdittedImageNode edittedImage,
			PreviewImageNode previewImage,
			DataNode data)
		{
			this.inputImage = inputImage;
			this.croppedImage = croppedImage;
			this.filteredImage = filteredImage;
			this.edittedImage = edittedImage;
			this.previewImage = previewImage;
			this.data = data;

			inputImage.Updated += (s, e) => RaisePropertyChanged(nameof(InputImage));
			inputImage.Outdated += (s, e) => RaisePropertyOutdated(nameof(InputImage));
			croppedImage.Updated += (s, e) => RaisePropertyChanged(nameof(CroppedImage));
			croppedImage.Outdated += (s, e) => RaisePropertyOutdated(nameof(CroppedImage));
			// the initialisation of EditService depends on the update of filteredImage!
			filteredImage.Updated += (s, e) => RaisePropertyChanged(nameof(FilteredImage));
			filteredImage.Outdated += (s, e) => RaisePropertyOutdated(nameof(FilteredImage));
			edittedImage.Updated += (s, e) => RaisePropertyChanged(nameof(EdittedImage));
			edittedImage.Outdated += (s, e) => RaisePropertyOutdated(nameof(EdittedImage));
			previewImage.Updated += (s, e) => RaisePropertyChanged(nameof(PreviewImage));
			previewImage.Outdated += (s, e) => RaisePropertyOutdated(nameof(PreviewImage));
			data.Updated += (s, e) => RaisePropertyChanged(nameof(Data));
			data.Outdated += (s, e) => RaisePropertyOutdated(nameof(Data));
		}
	}
}