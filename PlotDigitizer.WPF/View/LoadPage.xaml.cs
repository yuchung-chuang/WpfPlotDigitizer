using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using PlotDigitizer.Core;

namespace PlotDigitizer.WPF
{
    public partial class LoadPage : UserControl
    {
        public LoadPage()
        {
            InitializeComponent();
        }

        private void Page_DragOver(object sender, DragEventArgs e)
        {
            var isDropFile = e.Data.GetDataPresent(DataFormats.FileDrop)
                && File.Exists((e.Data.GetData(DataFormats.FileDrop) as string[])[0]);

            var isDropUrl = e.Data.GetDataPresent(DataFormats.Text)
                && e.Data.GetData(DataFormats.Text).ToString().ToUri() is not null;

            var isDropImage = e.Data.GetDataPresent(DataFormats.MetafilePicture);

            var isDropEnabled = isDropFile || isDropUrl || isDropImage;

            e.Effects = isDropEnabled ? DragDropEffects.Copy : DragDropEffects.None;
            if (!isDropEnabled) {
                e.Handled = true;
            }
        }
    }
}