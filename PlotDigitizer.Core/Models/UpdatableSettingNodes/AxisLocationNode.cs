namespace PlotDigitizer.Core
{
    public class AxisLocationNode : UpdatableNode<RectangleD>
	{
		public AxisLocationNode(InputImageNode inputImage)
        {
			DependsOn(inputImage);
        }

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated()) {
				return;
			}
			Data = default;
			OnUpdated();
		}
	}
}