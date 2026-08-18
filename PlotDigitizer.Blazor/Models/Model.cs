using Emgu.CV;
using Emgu.CV.Structure;

using PlotDigitizer.Core;

namespace PlotDigitizer.Blazor.Models
{
	/// <summary>
	/// Presentation adapter over the shared <see cref="UpdatableModel"/>. It adds nothing to the
	/// digitization itself; components read <see cref="InputImage"/>/<see cref="CroppedImage"/>/
	/// <see cref="FilteredImage"/> to know whether there is something to show, and the browser
	/// fetches the actual pixels from the <c>/image/{kind}</c> endpoint in <c>Program.cs</c> rather
	/// than through the component tree.
	///
	/// Unlike <c>PlotDigitizer.Web.Models.Model</c>, this spike does not need version-tokened image
	/// urls: a Blazor component re-renders its own markup whenever a bound field changes (here,
	/// after every slider edit), so the component can simply force a fresh `&lt;img&gt;` src with
	/// its own counter instead of tracking per-node version numbers.
	/// </summary>
	public class Model : UpdatableModel
	{
		public Model(InputImageNode inputImage,
			CroppedImageNode croppedImage,
			FilteredImageNode filteredImage,
			EditedImageNode editedImage,
			DataPointsNode dataPoints,
			DataNode data,
			Setting setting)
			: base(inputImage, croppedImage, filteredImage, editedImage, dataPoints, data)
		{
			Setting = setting;
		}

		public Setting Setting { get; }
	}
}
