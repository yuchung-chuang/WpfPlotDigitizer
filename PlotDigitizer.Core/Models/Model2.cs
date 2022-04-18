using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class Model2 : IModel
	{
		private readonly InputImageNode inputImage;
		private readonly CroppedImageNode croppedImage;
		private readonly FilteredImageNode filteredImage;
		private readonly EdittedImageNode edittedImage;
		private readonly PreviewImageNode previewImage;
		private readonly DataNode data;

		public Image<Rgba, byte> InputImage
		{
			get => inputImage.Get();
			set => inputImage.Set(value);
		}

		public Image<Rgba, byte> CroppedImage
		{
			get => croppedImage.Get();
			private set => croppedImage.Set(value);
		}

		public Image<Rgba, byte> FilteredImage
		{
			get => filteredImage.Get();
			private set => filteredImage.Set(value);
		}

		public Image<Rgba, byte> EdittedImage
		{
			get => edittedImage.Get();
			set => edittedImage.Set(value);
		}

		public Image<Rgba, byte> PreviewImage
		{
			get => previewImage.Get();
			private set => previewImage.Set(value);
		}

		public IEnumerable<PointD> Data
		{
			get => data.Get();
			private set => data.Set(value);
		}

		public Model2(InputImageNode inputImage,
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
		}
	}
}