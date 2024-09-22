using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class DataNode : UpdatableNode<IEnumerable<PointD>>
	{
		private readonly PreviewImageNode previewImage;
		private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;
		private readonly IImageService imageService;

		public DataNode(PreviewImageNode previewImage,
			AxisLimitNode axisLimit,
			AxisLogBaseNode axisLogBase,
			DataTypeNode dataType,
			IImageService imageService)
		{
			this.previewImage = previewImage;
			this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			this.imageService = imageService;
			previewImage.Updated += (s, e) => OnOutdated();
			previewImage.Outdated += (s, e) => OnOutdated();
			axisLimit.Updated += (s, e) => OnOutdated();
			axisLimit.Outdated += (s, e) => OnOutdated();
			axisLogBase.Updated += (s, e) => OnOutdated();
			axisLogBase.Outdated += (s, e) => OnOutdated();
			dataType.Updated += (s,e) => OnOutdated();
		}

		public override void Update()
		{
			if (!previewImage.CheckUpdate() || !axisLimit.CheckUpdate() || !axisLogBase.CheckUpdate())
				return;
			Value = previewImage.Value is null ? null : imageService.TransformData(previewImage.Points, previewImage.Value.Size, axisLimit.Value, axisLogBase.Value);
			base.Update();
		}
	}
}