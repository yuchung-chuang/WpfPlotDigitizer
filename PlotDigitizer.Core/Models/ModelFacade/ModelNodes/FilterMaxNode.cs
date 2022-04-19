using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilterMaxNode : ModelNode<Rgba> 
	{
		public FilterMaxNode() 
		{
			Value = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
			IsUpdated = true;
		}
	}
}
