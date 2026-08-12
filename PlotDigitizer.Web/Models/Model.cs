using Emgu.CV;
using Emgu.CV.Structure;

using PlotDigitizer.Core;

using System.Collections.Concurrent;

namespace PlotDigitizer.Web.Models
{
	/// <summary>
	/// Presentation adapter over the shared <see cref="UpdatableModel"/>. It adds nothing to the
	/// digitization itself; it only exposes the state in a form a Razor page can render, namely
	/// cache friendly image urls.
	/// </summary>
	public class Model : UpdatableModel
	{
		private readonly ConcurrentDictionary<string, int> versions = new();

		public Model(InputImageNode inputImage,
			CroppedImageNode croppedImage,
			FilteredImageNode filteredImage,
			EdittedImageNode edittedImage,
			DataPointsNode dataPoints,
			DataNode data,
			Setting setting)
			: base(inputImage, croppedImage, filteredImage, edittedImage, dataPoints, data)
		{
			Setting = setting;

			// A node update or invalidation changes what the image endpoint would return, so the
			// url has to change too, otherwise the browser serves a stale cached image.
			PropertyChanged += (s, e) => Bump(e.PropertyName);
			PropertyOutdated += (s, e) => Bump(e);
		}

		/// <summary>Width in css pixels used to display images in the workflow pages.</summary>
		public double DisplayWidth { get; } = 700d;

		public Setting Setting { get; }

		public string InputImageUrl => UrlFor(ImageKind.Input, InputImage);

		public string CroppedImageUrl => UrlFor(ImageKind.Cropped, CroppedImage);

		public string FilteredImageUrl => UrlFor(ImageKind.Filtered, FilteredImage);

		public string EdittedImageUrl => UrlFor(ImageKind.Editted, EdittedImage);

		public int VersionOf(string propertyName) =>
			versions.TryGetValue(propertyName, out var version) ? version : 0;

		private void Bump(string propertyName)
		{
			if (!string.IsNullOrEmpty(propertyName)) {
				versions.AddOrUpdate(propertyName, 1, (_, current) => current + 1);
			}
		}

		private string UrlFor(string kind, Image<Rgba, byte> image)
		{
			if (image is null) {
				return null;
			}

			var property = kind switch
			{
				ImageKind.Input => nameof(InputImage),
				ImageKind.Cropped => nameof(CroppedImage),
				ImageKind.Filtered => nameof(FilteredImage),
				_ => nameof(EdittedImage),
			};
			return $"/image/{kind}?v={VersionOf(property)}";
		}
	}
}
