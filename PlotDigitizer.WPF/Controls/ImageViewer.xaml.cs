using PropertyChanged;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.WPF
{
	[AddINotifyPropertyChangedInterface]
	public partial class ImageViewer : UserControl
	{
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ImageViewer), new PropertyMetadata());

		public ImageSource ImageSource
		{
			get => (ImageSource)GetValue(ImageSourceProperty);
			set => SetValue(ImageSourceProperty, value);
		}

		public ImageViewer()
		{
			InitializeComponent();
		}
	}
}