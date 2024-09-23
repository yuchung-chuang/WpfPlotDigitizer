using System.Globalization;
using System.Windows.Controls;

namespace PlotDigitizer.WPF
{
	public class NumberValidation : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
			=> new ValidationResult(
				string.IsNullOrWhiteSpace(value.ToString()) || double.TryParse(value.ToString(), out _),
				"Input value is not a double.");
	}
}