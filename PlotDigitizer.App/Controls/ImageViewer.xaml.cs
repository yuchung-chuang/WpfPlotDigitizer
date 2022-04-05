using PropertyChanged;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	[AddINotifyPropertyChangedInterface]
	public partial class ImageViewer : UserControl
	{
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata());

		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public ImageViewer()
		{
			InitializeComponent();
		}
	}
}