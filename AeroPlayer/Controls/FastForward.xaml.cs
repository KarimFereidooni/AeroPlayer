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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AeroPlayer.Controls
{
    /// <summary>
    /// Interaction logic for FastForward.xaml
    /// </summary>
    public partial class FastForward : UserControl
    {
        public FastForward()
        {
            InitializeComponent();
        }

        private MainWindow _Main_Window;
        public MainWindow Main_Window
        {
            get
            {
                return _Main_Window;
            }
            set
            {
                _Main_Window = value;
            }
        }

        private void my_image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,/Images/FastForwardP.png");
            bi.EndInit();
            my_image.Source = bi;
            Main_Window.FastForward();
            Program.CanDragMainWindow = false;
        }

        private void my_image_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri("pack://application:,,/Images/FastForward.png");
            bi.EndInit();
            my_image.Source = bi;
            Main_Window.Play();
            Program.CanDragMainWindow = true;
        }
    }
}
