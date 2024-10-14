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
			DependsOn(dataPoints);
			DependsOn(axisLimit);
			DependsOn(axisLogBase);
		}

		protected override void Update()
		{
			if (!IsAllDependenciesUpdated())
				return;
			Data = dataPoints.Data is null ? null : imageService.TransformData(dataPoints.Data, edittedImage.Data.Size, axisLimit.Data, axisLogBase.Data);
			OnUpdated();
		}
	}
}