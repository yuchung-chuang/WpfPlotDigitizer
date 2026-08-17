using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class DataNode : UpdatableNode<IEnumerable<PointD>>
	{
		private readonly DataPointsNode dataPoints;
        private readonly EditedImageNode editedImage;
        private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;
		private readonly IImageService imageService;

		public DataNode(EditedImageNode editedImage,
			AxisLimitNode axisLimit,
			AxisLogBaseNode axisLogBase,
            DataPointsNode dataPoints,
            IImageService imageService)
		{
            this.editedImage = editedImage;
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
			Data = dataPoints.Data is null || editedImage.Data is null
				? null
				: imageService.TransformData(dataPoints.Data, editedImage.Data.Size, axisLimit.Data, axisLogBase.Data);
		}
	}
}