using Microsoft.AspNetCore.Mvc;

using PlotDigitizer.Core;
using PlotDigitizer.Web.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PlotDigitizer.Web.Pages
{
	/// <summary>
	/// Web counterpart of <c>DataPageViewModel</c>: preview the extracted points over the edited
	/// image and export them. The desktop save dialog becomes a file download.
	/// </summary>
	public class DataPageModel : WorkflowPageModel
	{
		public DataPageModel(IDigitizerStateAccessor stateAccessor, WorkflowService workflow)
			: base(stateAccessor, workflow)
		{
		}

		public override WorkflowStep Step => WorkflowStep.Data;

		public IReadOnlyList<PointD> Data { get; private set; } = [];

		public bool IsAxisLimitSet => WorkflowService.HasAxisLimit(Setting);

		public void OnGet() => Load();

		public IActionResult OnPostDataType(DataType dataType)
		{
			Setting.DataType = dataType;
			Load();
			return Partial("_DataPageView", this);
		}

		public IActionResult OnGetExport(string format)
		{
			Load();
			if (Data.Count == 0) {
				return BadRequest("There is nothing to export.");
			}

			var isCsv = !string.Equals(format, "txt", StringComparison.OrdinalIgnoreCase);
			var separator = isCsv ? "," : "\t";
			var contentType = isCsv ? "text/csv" : "text/plain";
			var fileName = isCsv ? "plotdigitizer.csv" : "plotdigitizer.txt";

			return File(Encoding.UTF8.GetBytes(Serialize(separator)), contentType, fileName);
		}

		private void Load()
		{
			// The editor keeps its own copy of the image, so publish it before reading the points.
			State.CommitEdits();
			Data = Model.Data?.ToList() ?? [];
		}

		/// <summary>
		/// Same layout as the desktop export: a header built from the axis titles, then one row per
		/// point. Values are written with the invariant culture so a decimal comma never collides
		/// with a comma separator.
		/// </summary>
		private string Serialize(string separator)
		{
			var xLabel = string.IsNullOrWhiteSpace(Setting.AxisTitle.XLabel) ? "X" : Setting.AxisTitle.XLabel;
			var yLabel = string.IsNullOrWhiteSpace(Setting.AxisTitle.YLabel) ? "Y" : Setting.AxisTitle.YLabel;

			var content = new StringBuilder();
			content.AppendLine(Escape(xLabel, separator) + separator + Escape(yLabel, separator));
			foreach (var point in Data) {
				content.AppendLine(
					point.X.ToString(CultureInfo.InvariantCulture)
					+ separator
					+ point.Y.ToString(CultureInfo.InvariantCulture));
			}
			return content.ToString();
		}

		private static string Escape(string value, string separator)
		{
			if (separator != "," || !(value.Contains(',') || value.Contains('"'))) {
				return value;
			}
			return "\"" + value.Replace("\"", "\"\"") + "\"";
		}
	}
}
