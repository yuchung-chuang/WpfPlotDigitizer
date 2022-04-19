using System.Drawing;

namespace PlotDigitizer.Core
{
	public class AxisLocationNode : ModelNode<Rectangle>
	{
		private readonly InputImageNode inputImage;

		public AxisLocationNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate()) {
				return;
			}
			Value = Methods.GetAxisLocation(inputImage.Value) ??
				new Rectangle(
					inputImage.Value.Width / 4,
					inputImage.Value.Height / 4,
					inputImage.Value.Width / 2,
					inputImage.Value.Height / 2);
			OnUpdated();
		}
	}
}
