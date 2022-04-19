using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class CroppedImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly InputImageNode inputImage;
		private readonly AxisLocationNode axisLocation;

		public CroppedImageNode(InputImageNode inputImage, AxisLocationNode axisLocation)
		{
			this.inputImage = inputImage;
			this.axisLocation = axisLocation;
			inputImage.Updated += DependencyUpdated;
			axisLocation.Updated += DependencyUpdated;
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate() || !axisLocation.CheckUpdate())
				return;
			Value = Methods.CropImage(inputImage.Value, axisLocation.Value);
			OnUpdated();
		}
	}
}
