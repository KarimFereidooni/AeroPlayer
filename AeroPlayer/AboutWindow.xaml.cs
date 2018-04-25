using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AeroPlayer
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            AssemblyInformation info = new AssemblyInformation(System.Reflection.Assembly.GetExecutingAssembly());
            this.Title = string.Format("About {0}", info.AssemblyTitle);
            AboutLabel.Content = info.AssemblyTitle;
            AboutLabel.Content += Environment.NewLine;
            AboutLabel.Content += string.Format("Version {0}", info.AssemblyVersion);
            AboutLabel.Content += Environment.NewLine;
            AboutLabel.Content += info.AssemblyCompany;
            AboutLabel.Content += Environment.NewLine;
            AboutLabel.Content += info.AssemblyCopyright;
            AboutLabel.Content += Environment.NewLine;
            AboutLabel.Content += info.AssemblyDescription;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlassEffectHelper.EnableGlassEffect(this);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
