using System.Windows;
using System.Windows.Controls;

namespace PlotDigitizer.WPF
{
    public partial class AxLimTextBox : UserControl
    {
        #region Public Fields

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(AxLimTextBox), new PropertyMetadata(string.Empty));

        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(AxLimTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion Public Fields

        #region Public Constructors

        public AxLimTextBox()
        {
            InitializeComponent();
        }

        #endregion Public Constructors

        #region Public Properties

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion Public Properties
    }
}