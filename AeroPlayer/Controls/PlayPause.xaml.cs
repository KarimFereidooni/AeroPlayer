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
    /// Interaction logic for PlayPause.xaml
    /// </summary>
    public partial class PlayPause : UserControl
    {
        public PlayPause()
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
                _Main_Window.PlayStateChanged += new EventHandler<MainWindow.PlayStateChangedEventArgs>(Main_Window_PlayStateChanged);
            }
        }

        void Main_Window_PlayStateChanged(object sender, MainWindow.PlayStateChangedEventArgs e)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            if (e.State == MainWindow.Play_States.Playing)
            {
                if (Mouse_Down)
                    bi.UriSource = new Uri("pack://application:,,/Images/pauseP.png");
                else if (Mouse_Hover)
                    bi.UriSource = new Uri("pack://application:,,/Images/pauseH.png");
                else
                    bi.UriSource = new Uri("pack://application:,,/Images/pause.png");
            }
            else
            {
                if (Mouse_Down)
                    bi.UriSource = new Uri("pack://application:,,/Images/playP.png");
                else if (Mouse_Hover)
                    bi.UriSource = new Uri("pack://application:,,/Images/playH.png");
                else
                    bi.UriSource = new Uri("pack://application:,,/Images/play.png");
            }
            bi.EndInit();
            my_image.Source = bi;
        }

        private bool _Mouse_Hover = false;
        public bool Mouse_Hover
        {
            get
            {
                return _Mouse_Hover;
            }
            set
            {
                _Mouse_Hover = value;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (this.Main_Window.PlayState == MainWindow.Play_States.Playing)
                {
                    if (value)
                        bi.UriSource = new Uri("pack://application:,,/Images/pauseH.png");
                    else
                        bi.UriSource = new Uri("pack://application:,,/Images/pause.png");
                }
                else
                {
                    if (value)
                        bi.UriSource = new Uri("pack://application:,,/Images/playH.png");
                    else
                        bi.UriSource = new Uri("pack://application:,,/Images/play.png");
                }
                bi.EndInit();
                my_image.Source = bi;
            }
        }
        private bool _Mouse_Down = false;
        public bool Mouse_Down
        {
            get
            {
                return _Mouse_Down;
            }
            set
            {
                _Mouse_Down = value;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (this.Main_Window.PlayState == MainWindow.Play_States.Playing)
                {
                    if (value)
                        bi.UriSource = new Uri("pack://application:,,/Images/pauseP.png");
                    else
                        bi.UriSource = new Uri("pack://application:,,/Images/pause.png");
                }
                else
                {
                    if (value)
                        bi.UriSource = new Uri("pack://application:,,/Images/playP.png");
                    else
                        bi.UriSource = new Uri("pack://application:,,/Images/play.png");
                }
                bi.EndInit();
                my_image.Source = bi;
            }
        }

        private void my_image_MouseEnter(object sender, MouseEventArgs e)
        {
            
            this.Mouse_Hover = true;
        }

        private void my_image_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Mouse_Hover = false;
        }

        private void my_image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Mouse_Down = true;
            Program.CanDragMainWindow = false;
        }

        private void my_image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Mouse_Down = false;
            if (this.Main_Window.PlayState == MainWindow.Play_States.Playing)
                this.Main_Window.Pause();
            else
                this.Main_Window.Play();
            Program.CanDragMainWindow = true;
        }
    }
}
