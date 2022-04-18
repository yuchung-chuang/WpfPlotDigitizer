using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PlotDigitizer.Core
{
	public abstract class ModelNode<TData>
	{
		public virtual bool IsUpdated { get; set; } = false;
		public TData Value { get; set; }

		public event EventHandler Updated;

		protected virtual void OnUpdated()
		{
			Updated?.Invoke(this, EventArgs.Empty);
			IsUpdated = true;
		}

		public bool CheckUpdate()
		{
			if (!IsUpdated) {
				Update();
			}
			return IsUpdated;
		}

		public virtual void Update() { }

		public void Set(TData value)
		{
			Value = value;
			OnUpdated();
		}

		public TData Get()
		{
			CheckUpdate();
			return Value;
		}

		protected virtual void DependencyUpdated(object sender, EventArgs e)
		{
			IsUpdated = false;
		}
	}

	public class InputImageNode : ModelNode<Image<Rgba, byte>> { }

	public class AxisLocationNode : ModelNode<Rectangle>
	{
		private readonly InputImageNode inputImage;

		public AxisLocationNode(InputImageNode inputImage)
		{
			this.inputImage = inputImage;
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate()) {
				return;
			}
			Value = Methods.GetAxisLocation(inputImage.Value) ??
				new Rectangle(
					inputImage.Value.Width / 4,
					inputImage.Value.Height / 4,
					inputImage.Value.Width / 2,
					inputImage.Value.Height / 2);
			OnUpdated();
		}
	}

	public class CroppedImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly InputImageNode inputImage;
		private readonly AxisLocationNode axisLocation;

		public CroppedImageNode(InputImageNode inputImage, AxisLocationNode axisLocation)
		{
			this.inputImage = inputImage;
			this.axisLocation = axisLocation;
			inputImage.Updated += DependencyUpdated;
			axisLocation.Updated += DependencyUpdated;
		}

		public override void Update()
		{
			if (!inputImage.CheckUpdate() || !axisLocation.CheckUpdate())
				return;
			Value = Methods.CropImage(inputImage.Value, axisLocation.Value);
			OnUpdated();
		}
	}

	public class FilterMinNode : ModelNode<Rgba> 
	{
		public FilterMinNode()
		{
			Value = new Rgba(0, 0, 0, byte.MaxValue);
			IsUpdated = true;
		}
	}

	public class FilterMaxNode : ModelNode<Rgba> 
	{
		public FilterMaxNode() 
		{
			Value = new Rgba(byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue - 1, byte.MaxValue);
			IsUpdated = true;
		}
	}

	public class FilteredImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly CroppedImageNode croppedImage;
		private readonly FilterMinNode filterMin;
		private readonly FilterMaxNode filterMax;

		public FilteredImageNode(CroppedImageNode croppedImage, FilterMinNode filterMin, FilterMaxNode filterMax)
		{
			this.croppedImage = croppedImage;
			this.filterMin = filterMin;
			this.filterMax = filterMax;
			croppedImage.Updated += DependencyUpdated;
			filterMin.Updated += DependencyUpdated;
			filterMax.Updated += DependencyUpdated;
		}
		public override void Update()
		{
			if (!croppedImage.CheckUpdate() || !filterMin.CheckUpdate() || !filterMax.CheckUpdate()) {
				return;
			}
			Value = Methods.FilterRGB(croppedImage.Value, filterMin.Value, filterMax.Value);
			OnUpdated();
		}
	}
	public class EdittedImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly FilteredImageNode filteredImage;

		public EdittedImageNode(FilteredImageNode filteredImage)
		{
			this.filteredImage = filteredImage;
			filteredImage.Updated += DependencyUpdated;
		}

		public override void Update()
		{
			if (!filteredImage.CheckUpdate())
				return;
			Value = filteredImage.Value.Copy();
			OnUpdated();
		}
	}

	public class DataTypeNode : ModelNode<DataType> 
	{
		public DataTypeNode()
		{
			Value = DataType.Continuous;
			IsUpdated = true;
		}
	}
	public class PreviewImageNode : ModelNode<Image<Rgba, byte>>
	{
		private readonly EdittedImageNode edittedImage;
		private readonly DataTypeNode dataType;

		public IEnumerable<PointD> Points { get; private set; }

		public PreviewImageNode(EdittedImageNode edittedImage, DataTypeNode dataType)
		{
			this.edittedImage = edittedImage;
			this.dataType = dataType;
			edittedImage.Updated += DependencyUpdated;
			dataType.Updated += DependencyUpdated;
		}

		public override void Update()
		{
			if (!edittedImage.CheckUpdate() || !dataType.CheckUpdate())
				return;
			Value = edittedImage.Value.Copy();
			Points = dataType.Value switch
			{
				DataType.Discrete => Methods.GetDiscretePoints(Value),
				DataType.Continuous => Methods.GetContinuousPoints(Value),
				_ => throw new NotImplementedException(),
			};
			OnUpdated();
		}
	}
	public class AxisLimitNode : ModelNode<RectangleD> { }
	public class AxisLogBaseNode : ModelNode<PointD> 
	{
		public AxisLogBaseNode()
		{
			IsUpdated = true;
		}
	}

	public class DataNode : ModelNode<IEnumerable<PointD>>
	{
		private readonly PreviewImageNode previewImage;
		private readonly AxisLimitNode axisLimit;
		private readonly AxisLogBaseNode axisLogBase;

		public DataNode(PreviewImageNode previewImage, AxisLimitNode axisLimit, AxisLogBaseNode axisLogBase)
		{
			this.previewImage = previewImage;
			this.axisLimit = axisLimit;
			this.axisLogBase = axisLogBase;
			axisLimit.Updated += DependencyUpdated;
			axisLogBase.Updated += DependencyUpdated;
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
