using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilterMinNode : ModelNode<Rgba>
	{
		public FilterMinNode()
		{
			Value = new Rgba(0, 0, 0, byte.MaxValue);
			IsUpdated = true;
		}
	}
}
