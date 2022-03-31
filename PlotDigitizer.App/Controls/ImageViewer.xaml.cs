using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for ImageViewer.xaml
	/// </summary>
	public partial class ImageViewer : UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(UserControl), new PropertyMetadata());

		public event PropertyChangedEventHandler PropertyChanged;

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