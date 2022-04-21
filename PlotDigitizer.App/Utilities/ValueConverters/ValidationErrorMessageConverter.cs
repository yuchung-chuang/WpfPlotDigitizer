using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public class ValidationErrorMessageConverter : ValueConverterBase<ValidationErrorMessageConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is ReadOnlyCollection<ValidationError> errors)) {
				return null;
			}
			var sb = new StringBuilder();
			foreach (var e in errors.Where(e => e.ErrorContent != null)) {
				sb.AppendLine(e.ErrorContent.ToString());
			}

			return sb.ToString();
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}