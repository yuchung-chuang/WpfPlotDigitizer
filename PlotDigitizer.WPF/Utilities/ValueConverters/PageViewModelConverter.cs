using PlotDigitizer.Core;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace PlotDigitizer.WPF
{
	public class PageViewModelConverter : ValueConverterBase<PageViewModelConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is TabItem tabItem) 
				|| !(tabItem.Content is UserControl page) 
				|| !(page.DataContext is PageViewModelBase pageViewModel)) {
				return null;
			}
			return pageViewModel;
		}
	}
}
