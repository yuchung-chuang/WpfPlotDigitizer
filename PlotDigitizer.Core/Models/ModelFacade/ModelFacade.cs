using Emgu.CV;
using Emgu.CV.Structure;

using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class ModelFacade : Model
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
			protected set => croppedImage.Set(value);
		}

		public override Image<Rgba, byte> FilteredImage
		{
			get => filteredImage.Get();
			protected set => filteredImage.Set(value);
		}

		public override Image<Rgba, byte> EdittedImage
		{
			get => edittedImage.Get();
			set => edittedImage.Set(value);
		}

		public override Image<Rgba, byte> PreviewImage
		{
			get => previewImage.Get();
			protected set => previewImage.Set(value);
		}

		public override IEnumerable<PointD> Data
		{
			get => data.Get();
			protected set => data.Set(value);
		}

		public ModelFacade(InputImageNode inputImage,
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
			croppedImage.Updated += (s, e) => RaisePropertyChanged(nameof(CroppedImage));
			filteredImage.Updated += (s, e) => RaisePropertyChanged(nameof(FilteredImage));
			edittedImage.Updated += (s, e) => RaisePropertyChanged(nameof(EdittedImage));
			previewImage.Updated += (s, e) => RaisePropertyChanged(nameof(PreviewImage));
			data.Updated += (s, e) => RaisePropertyChanged(nameof(Data));
		}
	}
}