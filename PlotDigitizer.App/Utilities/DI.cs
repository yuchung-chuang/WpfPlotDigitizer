using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace PlotDigitizer.App
{
	public class DI : MarkupExtension
	{
		public static Func<Type, object> Resolver { get; set; }
		public Type Type { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) => Resolver?.Invoke(Type);
	}
}
