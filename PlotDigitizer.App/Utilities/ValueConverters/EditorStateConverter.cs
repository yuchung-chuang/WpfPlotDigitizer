using System;
using System.Globalization;
using System.Linq;

namespace PlotDigitizer.App
{
	public class EditorStateConverter : MultiValueConverterBase<EditorStateConverter>
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (!values.All(v => v is bool)) {
				return null;
			}
			var booleans = values.Select(v => (bool)v).ToList();
			if (booleans.Where(v => v == true).Count() > 1) {
				return null;
			}
			var index = booleans.IndexOf(true);
			return index switch
			{
				0 => EditorMode.PencilMode,
				1 => EditorMode.EraserMode,
				2 => EditorMode.RectMode,
				3 => EditorMode.PolyMode,
				_ => EditorMode.NoMode,
			};
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			if (!(value is EditorMode state)) {
				throw new NotImplementedException();
			}
			var booleans = new bool[4];
			switch (state) {
				case PencilMode _:
					booleans[0] = true;
					break;

				case EraserMode _:
					booleans[1] = true;
					break;

				case RectMode _:
					booleans[2] = true;
					break;

				case PolyMode _:
					booleans[3] = true;
					break;

				default:
				case NoMode _:
					break;
			}
			return booleans.Cast<object>().ToArray();
		}
	}
}