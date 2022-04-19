using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class DataNode : ModelNode<IEnumerable<PointD>>
	{
		private readonly PreviewImageNode previewImage;
		private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;

		

		public DataNode(PreviewImageNode previewImage, 
			AxisLimitNode axisLimit, 
			AxisLogBaseNode axisLogBase, 
			DataTypeNode dataType)
		{
			this.previewImage = previewImage;
			this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			previewImage.Updated += DependencyUpdated;
			axisLimit.Updated += DependencyUpdated;
			axisLogBase.Updated += DependencyUpdated;
			dataType.Updated += DependencyUpdated;
		}

		public override void Update()
		{
			if (!previewImage.CheckUpdate() || !axisLimit.CheckUpdate() || !axisLogBase.CheckUpdate())
				return;
			Value = Methods.TransformData(previewImage.Points, previewImage.Value.Size, axisLimit.Value, axisLogBase.Value);
			OnUpdated();
		}
	}
}
