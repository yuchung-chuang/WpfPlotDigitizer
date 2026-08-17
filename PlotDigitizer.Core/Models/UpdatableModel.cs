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
		private readonly EditedImageNode editedImage;
		private readonly DataPointsNode dataPoints;
		private readonly DataNode data;

		public override Image<Rgba, byte> InputImage
		{
			get => inputImage.GetUpdatedData();
			set => inputImage.Data = value;
		}

		public override Image<Rgba, byte> CroppedImage
		{
			get => croppedImage.GetUpdatedData();
			set => croppedImage.Data = value;
		}

		public override Image<Rgba, byte> FilteredImage
		{
			get => filteredImage.GetUpdatedData();
			set => filteredImage.Data = value;
		}

		public override Image<Rgba, byte> EditedImage
		{
			get => editedImage.GetUpdatedData();
			set => editedImage.Data = value;
		}

		public override IEnumerable<PointD> DataPoints
		{
			get => dataPoints.GetUpdatedData();
			set => dataPoints.Data = value;
		}

		public override IEnumerable<PointD> Data
		{
			get => data.GetUpdatedData();
			set => data.Data = value;
		}

		public UpdatableModel(InputImageNode inputImage,
			CroppedImageNode croppedImage,
			FilteredImageNode filteredImage,
			EditedImageNode editedImage,
			DataPointsNode dataPoints,
			DataNode data)
        {
            this.inputImage = inputImage;
            this.croppedImage = croppedImage;
            this.filteredImage = filteredImage;
            this.editedImage = editedImage;
            this.dataPoints = dataPoints;
            this.data = data;

            RelayEvents(inputImage, nameof(InputImage));
			RelayEvents(croppedImage, nameof(CroppedImage));
			RelayEvents(filteredImage, nameof(FilteredImage));
			RelayEvents(editedImage, nameof(EditedImage));
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