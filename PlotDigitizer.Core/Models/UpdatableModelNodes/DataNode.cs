using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class DataNode : UpdatableNode<IEnumerable<PointD>>
	{
		private readonly DataPointsNode dataPoints;
        private readonly EdittedImageNode edittedImage;
        private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;
		private readonly IImageService imageService;

		public DataNode(EdittedImageNode edittedImage,
			AxisLimitNode axisLimit,
			AxisLogBaseNode axisLogBase,
            DataPointsNode dataPoints,
            IImageService imageService)
		{
            this.edittedImage = edittedImage;
            this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			this.dataPoints = dataPoints;
			this.imageService = imageService;
			dataPoints.Updated += (s, e) => OnOutdated();
			dataPoints.Outdated += (s, e) => OnOutdated();
			axisLimit.Updated += (s, e) => OnOutdated();
			axisLimit.Outdated += (s, e) => OnOutdated();
			axisLogBase.Updated += (s, e) => OnOutdated();
			axisLogBase.Outdated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!dataPoints.CheckUpdate() || !axisLimit.CheckUpdate() || !axisLogBase.CheckUpdate())
				return;
			Value = dataPoints.Value is null ? null : imageService.TransformData(dataPoints.Value, edittedImage.Value.Size, axisLimit.Value, axisLogBase.Value);
			base.Update();
		}
	}
}