namespace PlotDigitizer.Core
{
	public class AxisLogBaseNode : UpdatableNode<PointD>
	{
		public AxisLogBaseNode(InputImageNode inputImage)
		{
			DependsOn(inputImage);
            IsUpdated = true;
        }

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated())
				return;
			Data = default;
			OnUpdated();
		}
	}
}