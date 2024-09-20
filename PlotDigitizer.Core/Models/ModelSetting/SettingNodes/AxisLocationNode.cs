using System.Drawing;

namespace PlotDigitizer.Core
{
	public class AxisLocationNode : ModelNode<RectangleD>
	{
		private readonly InputImageNode inputImage;

		public AxisLocationNode(InputImageNode inputImage)
        {
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			Value = default;
			base.Update();
		}
	}
}