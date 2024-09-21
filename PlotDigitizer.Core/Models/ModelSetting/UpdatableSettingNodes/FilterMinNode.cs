using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilterMinNode : UpdatableNode<Rgba>
	{
		private readonly InputImageNode inputImage;

		public FilterMinNode(InputImageNode inputImage)
		{
			Value = new Rgba(0, 0, 0, byte.MaxValue);
			IsUpdated = true;
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
		}
		public override void Update()
		{
			base.Update();
			Value = new Rgba(0, 0, 0, byte.MaxValue);
		}
	}
}