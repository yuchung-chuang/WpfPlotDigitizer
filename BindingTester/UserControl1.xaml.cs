using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BindingTester
{
  /// <summary>
  /// UserControl1.xaml 的互動邏輯
  /// </summary>
  public partial class UserControl1 : UserControl, INotifyPropertyChanged
  {
    public UserControl1()
    {
      InitializeComponent();
      grid.DataContext = this;
      PropertyChanged += UserControl1_PropertyChanged;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void UserControl1_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Text))
      {
        Debug.WriteLine("PropertyChanged");
      }
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == TextProperty)
      {
        Debug.WriteLine("Override Dependency Property Changed");
      }
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      nameof(Text),
      typeof(string),
      typeof(UserControl1),
      new PropertyMetadata(default(string), TextChangedCallback));

    private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      Debug.WriteLine("Dependency Property Changed");
      (d as UserControl1).OnPropertyChanged(e.Property.Name);
    }

    //Do not add any logic to dependency property wrappers, because they are only called when you set the property from code. If you set the property from XAML the SetValue() method is called directly.
    public string Text
    {
      get => GetValue(TextProperty) as string;
      set => SetValue(TextProperty, value);
    }
  }
}
