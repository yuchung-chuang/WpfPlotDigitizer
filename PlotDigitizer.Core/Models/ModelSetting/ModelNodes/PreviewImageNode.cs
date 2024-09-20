using Emgu.CV;
using Emgu.CV.Structure;

using System;
using System.Collections.Generic;

namespace PlotDigitizer.Core
{
	public class PreviewImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly EdittedImageNode edittedImage;
		private readonly DataTypeNode dataType;
		private readonly IImageService imageService;

		public IEnumerable<PointD> Points { get; private set; }

		public override bool IsUpdated
		{
			get => base.IsUpdated;
			set => base.IsUpdated = value;
		}

		public PreviewImageNode(EdittedImageNode edittedImage, 
			DataTypeNode dataType,
			IImageService imageService)
		{
			this.edittedImage = edittedImage;
			this.dataType = dataType;
			this.imageService = imageService;
			edittedImage.Updated += (s, e) => OnOutdated();
			edittedImage.Outdated += (s, e) => OnOutdated();
			dataType.Updated += (s, e) => OnOutdated();
			dataType.Outdated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			if (!edittedImage.CheckUpdate() || !dataType.CheckUpdate())
				return;
			Value = edittedImage.Value?.Copy();
			Points = dataType.Value switch
			{
				DataType.Discrete => imageService.GetDiscretePoints(Value),
				DataType.Continuous => imageService.GetContinuousPoints(Value),
				_ => throw new NotImplementedException(),
			};
			base.Update();
		}
	}
}