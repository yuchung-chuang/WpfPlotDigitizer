using Emgu.CV;
using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilteredImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly CroppedImageNode croppedImage;
		private readonly FilterMinNode filterMin;
		private readonly FilterMaxNode filterMax;

		public FilteredImageNode(CroppedImageNode croppedImage, FilterMinNode filterMin, FilterMaxNode filterMax)
		{
			this.croppedImage = croppedImage;
			this.filterMin = filterMin;
			this.filterMax = filterMax;
			croppedImage.Updated += DependencyUpdated;
			filterMin.Updated += DependencyUpdated;
			filterMax.Updated += DependencyUpdated;
		}
		public override void Update()
		{
			if (!croppedImage.CheckUpdate() || !filterMin.CheckUpdate() || !filterMax.CheckUpdate()) {
				return;
			}
			Value = Methods.FilterRGB(croppedImage.Value, filterMin.Value, filterMax.Value);
			OnUpdated();
		}
	}
}
