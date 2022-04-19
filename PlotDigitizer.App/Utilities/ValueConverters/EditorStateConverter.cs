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
				throw new NotImplementedException();
			}
			var booleans = values.Select(v => (bool)v).ToList();
			if (booleans.Where(v => v == true).Count() > 1) {
				throw new NotImplementedException();
			}
			var index = booleans.IndexOf(true);
			return index switch
			{
				0 => PencilMode.Instance,
				1 => EraserMode.Instance,
				2 => RectMode.Instance,
				3 => PolyMode.Instance,
				_ => NoMode.Instance,
			};
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			if (!(value is EditorState state)) {
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
