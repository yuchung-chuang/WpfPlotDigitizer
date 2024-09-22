﻿using Emgu.CV.Structure;

namespace PlotDigitizer.Core
{
	public class FilterMaxNode : UpdatableNode<Rgba>
	{
		private readonly InputImageNode inputImage;

		public FilterMaxNode(InputImageNode inputImage)
		{
			Value = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
			IsUpdated = true;
			this.inputImage = inputImage;
			inputImage.Updated += (s, e) => OnOutdated();
		}

		public override void Update()
		{
			Value = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
			base.Update();
		}
	}
}