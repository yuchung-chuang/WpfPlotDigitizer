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
		private readonly DataPointsNode dataPoints;
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

		public override IEnumerable<PointD> DataPoints
		{
			get => dataPoints.Get();
			set => dataPoints.Set(value);
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
			DataPointsNode dataPoints,
			DataNode data)
        {
            this.inputImage = inputImage;
            this.croppedImage = croppedImage;
            this.filteredImage = filteredImage;
            this.edittedImage = edittedImage;
            this.dataPoints = dataPoints;
            this.data = data;

            RelayEvents(inputImage, nameof(InputImage));
			RelayEvents(croppedImage, nameof(CroppedImage));
			RelayEvents(filteredImage, nameof(FilteredImage));
			RelayEvents(edittedImage, nameof(EdittedImage));
			RelayEvents(dataPoints, nameof(DataPoints));
			RelayEvents(data, nameof(Data));
        }

        private void RelayEvents(UpdatableNode node, string propertyName)
        {
            node.Updated += (s, e) => OnPropertyChanged(propertyName);
            node.Outdated += (s, e) => OnPropertyOutdated(propertyName);
        }
    }
}