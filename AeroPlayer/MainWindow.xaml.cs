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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace AeroPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyMediaElement.LoadedBehavior = MediaState.Manual;
            MyMediaElement.UnloadedBehavior = MediaState.Manual;
            playPause_button.Main_Window = this;
            stop_button.Main_Window = this;
            next_button.Main_Window = this;
            previous_button.Main_Window = this;
            fastForward_button.Main_Window = this;
            reWind_button.Main_Window = this;
            PositionTimer = new System.Windows.Forms.Timer();
            StatusTimer = new System.Windows.Forms.Timer();
            MouseTimer = new System.Windows.Forms.Timer();
            PositionTimer.Interval = 1000;
            StatusTimer.Interval = 2000;
            MouseTimer.Interval = 2000;
            PositionTimer.Enabled = false;
            StatusTimer.Enabled = false;
            MouseTimer.Enabled = false;
            PositionTimer.Tick += new EventHandler(PositionTimer_Tick);
            StatusTimer.Tick += new EventHandler(StatusTimer_Tick);
            MouseTimer.Tick += new EventHandler(MouseTimer_Tick);
            this.PlayState = Play_States.Ready;
            this.RepeatState = Repeat_States.RepeatAll;
            LoadSetting();
        }

        void MouseTimer_Tick(object sender, EventArgs e)
        {
            if (this.IsMouseOver)
            {
                HideCursor();
            }
            this.MouseTimer.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlassEffectHelper.EnableGlassEffect(this);
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                if (System.IO.File.Exists(args[i]))
                    PlayList.AddToPlayList(new Media(args[i]));
            }
            if (PlayList.Count > 0)
                PlayFromPlayList(0);
            HwndSource hwndSource = HwndSource.FromHwnd(this.Handle);
            hwndSource.AddHook(new HwndSourceHook(WndProc));
            if (Windows7Taskbar.Windows7OrGreater)
            {
                ThumbnailToolBarButton thumbButtonPlay = new ThumbnailToolBarButton(Properties.Resources.PlayIcon, "Play/Pause");
                thumbButtonPlay.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(thumbButtonPlay_Click);
                ThumbnailToolBarButton thumbButtonNext = new ThumbnailToolBarButton(Properties.Resources.NextIcon, "Next");
                thumbButtonNext.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(thumbButtonNext_Click);
                ThumbnailToolBarButton thumbButtonPrev = new ThumbnailToolBarButton(Properties.Resources.PrevIcon, "Previous");
                thumbButtonPrev.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(thumbButtonPrev_Click);
                TaskbarManager.Instance.ThumbnailToolBars.AddButtons(this.Handle, thumbButtonPrev, thumbButtonPlay, thumbButtonNext);
                //TaskbarManager.Instance.TabbedThumbnail.SetThumbnailClip(this.Handle, new System.Drawing.Rectangle((int)this.MyMediaElement.Margin.Left, (int)this.MyMediaElement.Margin.Top, (int)this.Width, (int)this.MyMediaElement.Height));
            }
        }

        void thumbButtonPrev_Click(object sender, ThumbnailButtonClickedEventArgs e)
        {
            this.PlayPrev();
        }

        void thumbButtonNext_Click(object sender, ThumbnailButtonClickedEventArgs e)
        {
            this.PlayNext();
        }

        void thumbButtonPlay_Click(object sender, ThumbnailButtonClickedEventArgs e)
        {
            this.PlayPause();
        }

        void StatusTimer_Tick(object sender, EventArgs e)
        {
            StatusTextBlock.Text = "";
            StatusTimer.Stop();
        }

        void PositionTimer_Tick(object sender, EventArgs e)
        {
            mediaSlider.Value = MyMediaElement.Position.TotalSeconds;
        }

        private string GetPositionString(TimeSpan timeSpan)
        {
            return timeSpan.Hours.ToString() + ":" + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString();
        }

        private System.Windows.Forms.Timer _PositionTimer;
        public System.Windows.Forms.Timer PositionTimer
        {
            get
            {
                return _PositionTimer;
            }
            set
            {
                _PositionTimer = value;
            }
        }

        private System.Windows.Forms.Timer _StatusTimer;
        public System.Windows.Forms.Timer StatusTimer
        {
            get
            {
                return _StatusTimer;
            }
            set
            {
                _StatusTimer = value;
            }
        }

        private System.Windows.Forms.Timer _MouseTimer;
        public System.Windows.Forms.Timer MouseTimer
        {
            get
            {
                return _MouseTimer;
            }
            set
            {
                _MouseTimer = value;
            }
        }

        private PlayListManager _PlayList;
        public PlayListManager PlayList
        {
            get
            {
                if (_PlayList == null)
                    _PlayList = new PlayListManager();
                return _PlayList;
            }
            set
            {
                _PlayList = value;
            }
        }

        public enum Play_States
        {
            Playing,
            Paused,
            Stoped,
            Ready
        }

        public enum Repeat_States
        {
            NoRepeat = 1,
            RepeatOne = 2,
            RepeatAll = 3
        }

        private Play_States _PlayState;
        public Play_States PlayState
        {
            get
            {
                return _PlayState;
            }
            set
            {
                _PlayState = value;
                OnPlayStateChanged(value);
                SetStatusText();
                if (value == Play_States.Playing)
                    Windows7Taskbar.SetProgressState(this.Handle, ThumbnailProgressState.Indeterminate);
                else if (value == Play_States.Paused)
                    Windows7Taskbar.SetProgressState(this.Handle, ThumbnailProgressState.Paused);
                else if (value == Play_States.Stoped)
                    Windows7Taskbar.SetProgressState(this.Handle, ThumbnailProgressState.Error);
                else
                    Windows7Taskbar.SetProgressState(this.Handle, ThumbnailProgressState.NoProgress);
            }
        }

        public IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }

        private Repeat_States _RepeatState;
        public Repeat_States RepeatState
        {
            get
            {
                return _RepeatState;
            }
            set
            {
                _RepeatState = value;
            }
        }

        private void OnPlayStateChanged(Play_States value)
        {
            if (this.PlayStateChanged != null)
                this.PlayStateChanged(this, new PlayStateChangedEventArgs(value));
        }

        public class PlayStateChangedEventArgs : EventArgs
        {
            private Play_States _State;
            public Play_States State
            {
                get
                {
                    return _State;
                }
                set
                {
                    _State = value;
                }
            }
            public PlayStateChangedEventArgs(Play_States State)
            {
                this.State = State;
            }
        }

        public event EventHandler<PlayStateChangedEventArgs> PlayStateChanged;

        public MediaElement Media_Element
        {
            get
            {
                return this.MyMediaElement;
            }
        }

        public void Pause()
        {
            if (this.PlayState == Play_States.Playing)
            {
                this.MyMediaElement.Pause();
                this.PlayState = Play_States.Paused;
                PositionTimer.Stop();
            }
        }

        public void PlayPause()
        {
            if (this.PlayState == Play_States.Playing)
                Pause();
            else
                Play();
        }

        public void Play()
        {
            if (this.MyMediaElement.Source == null)
                OpenMenu_Click(this, new RoutedEventArgs());
            else
            {
                this.MyMediaElement.Play();
                this.MyMediaElement.SpeedRatio = 1;
                this.PlayState = Play_States.Playing;
                PositionTimer.Start();
            }
        }

        public void Open(Media media)
        {
            MyMediaElement.Source = new Uri(media.FilePath);
            MyMediaElement.Play();
        }

        public void Stop()
        {
            this.MyMediaElement.Stop();
            this.PlayState = Play_States.Stoped;
            PositionTimer.Stop();
        }

        private void MyMediaElement_BufferingStarted(object sender, RoutedEventArgs e)
        {
            SetStatusText("BufferingStarted");
        }

        private void MyMediaElement_BufferingEnded(object sender, RoutedEventArgs e)
        {
            SetStatusText("BufferingEnded");
        }

        private void PlayFirst()
        {
            if (PlayList.Count > 0)
                Open(PlayList[0]);
        }
        private void PlayLast()
        {
            if (PlayList.Count > 0)
                Open(PlayList[PlayList.Count - 1]);
        }
        private void PlayAgain()
        {
            if (MyMediaElement.Source != null)
            {
                Media media = this.PlayList[MyMediaElement.Source.LocalPath];
                if (media != null)
                    Open(media);
            }
        }

        private void MyMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (this.RepeatState)
            {
                case Repeat_States.NoRepeat:
                    if (!PlayNext())
                        Stop();
                    break;
                case Repeat_States.RepeatOne:
                    Stop();
                    PlayAgain();
                    break;
                case Repeat_States.RepeatAll:
                    if (!PlayNext())
                        PlayFirst();
                    break;
            }
        }

        private void MyMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
            Stop();
        }

        private void MyMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MyMediaElement.HasVideo)
                logo_image.Visibility = Visibility.Collapsed;
            else
                logo_image.Visibility = Visibility.Visible;
            try
            {
                mediaSlider.Minimum = 0;
                mediaSlider.Maximum = MyMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
            catch
            {
            }
            this.PlayState = Play_States.Playing;
            PositionTimer.Start();
            if (Properties.Settings.Default.FitPlayerToVideo)
            {
                if (MyMediaElement.HasVideo)
                {
                    this.Width = MyMediaElement.NaturalVideoWidth;
                    this.Height = MyMediaElement.NaturalVideoHeight + this.MinHeight;
                }
                else
                {
                    this.Width = this.MinWidth;
                    this.Height = this.MinHeight;
                }
            }
        }

        Thickness OldMargin;
        Stretch OldStretch;
        System.Drawing.RectangleF OldLocation;

        private bool _FullScreen = false;
        public bool FullScreen
        {
            get
            {
                return _FullScreen;
            }
            set
            {
                _FullScreen = value;
                if (value)
                {
                    //this.Hide();
                    this.Topmost = true;
                    this.WindowStyle = WindowStyle.None;
                    OldLocation = new System.Drawing.RectangleF((float)this.Left, (float)this.Top, (float)this.Width, (float)this.Height);
                    this.Top = 0;
                    this.Left = 0;
                    this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                    this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                    this.WindowState = WindowState.Maximized;
                    OldStretch = this.MyMediaElement.Stretch;
                    this.MyMediaElement.Stretch = Stretch.Fill;
                    OldMargin = this.MyMediaElement.Margin;
                    this.MyMediaElement.Margin = new Thickness(0);
                    mediaPanel.Visibility = Visibility.Collapsed;
                    this.MouseTimer.Start();
                    //this.Show();
                }
                else
                {
                    //this.Hide();
                    this.WindowState = WindowState.Normal;
                    this.Topmost = false;
                    this.MyMediaElement.Margin = OldMargin;
                    this.MyMediaElement.Stretch = OldStretch;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.Top = OldLocation.Y;
                    this.Left = OldLocation.X;
                    this.Width = OldLocation.Width;
                    this.Height = OldLocation.Height;
                    mediaPanel.Visibility = Visibility.Visible;
                    this.MouseTimer.Stop();
                    //this.Show();
                    ShowCursor();
                }
            }
        }

        public event EventHandler DoubleClick;
        private void OnDoubleClick()
        {
            if (this.DoubleClick != null)
            {
                DoubleClick(this, EventArgs.Empty);
            }
        }

        private void SetPosition(double position)
        {
            SetPosition(TimeSpan.FromSeconds(position));
        }
        private void SetPosition(TimeSpan position)
        {
            MyMediaElement.Position = position;
            SetStatusText("Position " + position.Hours.ToString() + ":" + position.Minutes.ToString() + ":" + position.Seconds.ToString());
        }

        private Microsoft.Win32.OpenFileDialog _OpenDialog;
        public Microsoft.Win32.OpenFileDialog OpenDialog
        {
            get
            {
                if (_OpenDialog == null)
                {
                    _OpenDialog = new Microsoft.Win32.OpenFileDialog();
                    _OpenDialog.Multiselect = true;

                    //string audioExtensionLine = "*.wma; *.mp3; *.midi; *.wav; *.mka; *.mp4a";
                    //string videoExtensionLine = "*.asf; *.wmv; *.cda; *.avi; *.mpeg; *.aiff; *.au; *.mkv; *.flv; *.mp4v; *.dat; *.mp4";
                    _OpenDialog.Filter = "All files|*.*"; //String.Format("Video and audio files|{0};{1}|Video files|{0}|Audio files|{1}|All files|*.*", videoExtensionLine, audioExtensionLine);
                }
                return _OpenDialog;
            }
            set
            {
                _OpenDialog = value;
            }
        }
        //private System.Windows.Forms.OpenFileDialog _OpenDialog;
        //public System.Windows.Forms.OpenFileDialog OpenDialog
        //{
        //    get
        //    {
        //        if (_OpenDialog == null)
        //        {
        //            _OpenDialog = new System.Windows.Forms.OpenFileDialog();
        //            _OpenDialog.Multiselect = true;
        //        }
        //        return _OpenDialog;
        //    }
        //    set
        //    {
        //        _OpenDialog = value;
        //    }
        //}

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            bool? result = OpenDialog.ShowDialog(this);
            if (result.HasValue && result.Value == true)
            {
                PlayList.ClearPlayList();
                foreach (var item in OpenDialog.FileNames)
                {
                    PlayList.AddToPlayList(new Media(item));
                }
                PlayFromPlayList(0);
            }
            //System.Windows.Forms.DialogResult result = OpenDialog.ShowDialog();
            //if (result == System.Windows.Forms.DialogResult.OK)
            //{
            //    PlayList.ClearPlayList();
            //    foreach (var item in OpenDialog.FileNames)
            //    {
            //        PlayList.AddToPlayList(item);
            //    }
            //    PlayFromPlayList(0);
            //}
        }

        public void PlayFromPlayList(int index)
        {
            if (PlayList.Count > index)
            {
                Open(PlayList[index]);
            }
        }

        public bool PlayNext()
        {
            if (MyMediaElement.Source == null)
                return false;
            Media current_media = this.PlayList[MyMediaElement.Source.LocalPath];
            if (current_media != null)
                if ((PlayList.IndexOf(current_media) + 1) < PlayList.Count)
                {
                    Open(PlayList[PlayList.IndexOf(current_media) + 1]);
                    return true;
                }
            return false;
        }
        public bool PlayPrev()
        {
            if (MyMediaElement.Source == null)
                return false;
            Media current_media = this.PlayList[MyMediaElement.Source.LocalPath];
            if (current_media != null)
                if ((PlayList.IndexOf(current_media) - 1) >= 0)
                {
                    Open(PlayList[PlayList.IndexOf(current_media) - 1]);
                    return true;
                }
            return false;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                PlayPause();
            }
            else if (e.Key == Key.Enter)
            {
                this.FullScreen = !this.FullScreen;
            }
            else if (e.Key == Key.Escape)
            {
                this.Stop();
            }
            else if (e.Key == Key.Left)
            {
                SetPosition(MyMediaElement.Position.TotalSeconds - 5);
            }
            else if (e.Key == Key.Right)
            {
                SetPosition(MyMediaElement.Position.TotalSeconds + 5);
            }
            else if (e.Key == Key.Up)
            {
                SetVolume(MyMediaElement.Volume + .02);
            }
            else if (e.Key == Key.Down)
            {
                SetVolume(MyMediaElement.Volume - .02);
            }
            else if (e.Key == Key.PageDown)
            {
                PlayNext();
            }
            else if (e.Key == Key.PageUp)
            {
                PlayPrev();
            }
        }

        public void SetVolume(double value)
        {
            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;
            MyMediaElement.Volume = value;
            if (VolumeSlider.Value != value)
                VolumeSlider.Value = value;
            SetStatusText("Volume " + ((int)(value * 100)).ToString());
        }

        public void SetStatusText(string text)
        {
            StatusTimer.Enabled = false;
            StatusTextBlock.Text = text;
            StatusTimer.Start();
        }
        public void SetStatusText()
        {
            if (PlayState == Play_States.Playing)
            {
                StatusLable.Content = PlayState.ToString() + " " + PlayList[MyMediaElement.Source.LocalPath].Name;
            }
            else
                StatusLable.Content = PlayState.ToString();
        }

        private void mediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Positionlabel.Content = GetPositionString(TimeSpan.FromSeconds(e.NewValue)) + ((MyMediaElement.NaturalDuration.HasTimeSpan) ? (" / " + GetPositionString(MyMediaElement.NaturalDuration.TimeSpan)) : "");
            Windows7Taskbar.SetProgressValue(this.Handle, (ulong)mediaSlider.Value, (ulong)mediaSlider.Maximum);
            if (A.HasValue && B.HasValue)
            {
                if (TimeSpan.Compare(MyMediaElement.Position, B.Value) >= 0)
                    SetPosition(A.Value);
            }
        }

        private void mediaSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                SetPosition(mediaSlider.Value);
                if (PlayState == Play_States.Playing)
                    PositionTimer.Start();
            }
        }

        private void mediaSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                PositionTimer.Stop();
            }
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Properties.Settings.Default.SetVolumeOnScroll)
            {
                if (e.Delta > 0)
                    SetVolume(MyMediaElement.Volume + .03);
                else if (e.Delta < 0)
                    SetVolume(MyMediaElement.Volume - .03);
            }
            else
            {
                if (e.Delta > 0)
                    SetPosition(MyMediaElement.Position.TotalSeconds + 5);
                else if (e.Delta < 0)
                    SetPosition(MyMediaElement.Position.TotalSeconds - 5);
            }
        }

        public string MakeSnapshot()
        {
            string path = System.IO.Path.GetTempFileName();
            Size mediaElementSize = new Size(this.MyMediaElement.ActualWidth, this.MyMediaElement.ActualHeight);
            Size dpi = new Size(96, 96);
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)mediaElementSize.Width, (int)mediaElementSize.Height, dpi.Width, dpi.Height, PixelFormats.Pbgra32);
            bitmap.Render(this.MyMediaElement);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Interlace = PngInterlaceOption.On;
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                stream.Flush();
            }
            using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.PhotoSound))
                player.Play();
            return path;
        }

        DateTime? LastMouseButtonDownTime = null;
        private void MyMediaElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!LastMouseButtonDownTime.HasValue)
                LastMouseButtonDownTime = DateTime.Now;
            else
            {
                TimeSpan t = DateTime.Now.Subtract(LastMouseButtonDownTime.Value);
                if (t.TotalMilliseconds <= System.Windows.Forms.SystemInformation.DoubleClickTime)
                {
                    OnDoubleClick();
                    this.FullScreen = !this.FullScreen;
                    LastMouseButtonDownTime = null;
                }
                else
                {
                    LastMouseButtonDownTime = DateTime.Now;
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Program.CanDragMainWindow)
                this.DragMove();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                string[] files = (string[])e.Data.GetData("FileDrop");
                PlayList.ClearPlayList();
                foreach (var item in files)
                {
                    PlayList.AddToPlayList(new Media(item));
                }
                PlayFromPlayList(0);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_COPYDATA:
                    Win32.DataStruct _DataStruct = (Win32.DataStruct)Marshal.PtrToStructure(lParam, typeof(Win32.DataStruct));
                    PlayFromPlayList(PlayList.AddToPlayList(new Media(Program.DecodeString(_DataStruct.LP))));
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ViewNormalMenu_Click(object sender, RoutedEventArgs e)
        {
            if (this.FullScreen)
                this.FullScreen = false;
        }

        private void ViewFullScreenMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!this.FullScreen)
                this.FullScreen = true;
        }

        private void PlayPauseMenu_Click(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }

        private void StopMenu_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void PlayFirstMenu_Click(object sender, RoutedEventArgs e)
        {
            PlayFirst();
        }

        private void PlayNextMenu_Click(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        private void PlayPrevMenu_Click(object sender, RoutedEventArgs e)
        {
            PlayPrev();
        }

        private void PlayLastMenu_Click(object sender, RoutedEventArgs e)
        {
            PlayLast();
        }

        private void PlayFastMenu_Click(object sender, RoutedEventArgs e)
        {
            FastForward();
        }

        private void PlayNormalMenu_Click(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void PlaySlowMenu_Click(object sender, RoutedEventArgs e)
        {
            ReWind();
        }

        private void FitPlayerToVideoMenu_Click(object sender, RoutedEventArgs e)
        {
            if (FitPlayerToVideoMenu.IsChecked)
            {
                Menu100_Click(sender, e);
            }
        }

        private void Menu100_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaElement.HasVideo)
            {
                this.Width = MyMediaElement.NaturalVideoWidth;
                this.Height = MyMediaElement.NaturalVideoHeight + this.MinHeight;
            }
        }

        private void Menu50_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaElement.HasVideo)
            {
                this.Width = MyMediaElement.NaturalVideoWidth / 2;
                this.Height = MyMediaElement.NaturalVideoHeight / 2 + this.MinHeight;
            }
        }

        private void Menu200_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaElement.HasVideo)
            {
                this.Width = MyMediaElement.NaturalVideoWidth * 2;
                this.Height = MyMediaElement.NaturalVideoHeight * 2 + this.MinHeight;
            }
        }

        private void VideoMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.FitPlayerToVideo)
                FitPlayerToVideoMenu.IsChecked = true;
            else
                FitPlayerToVideoMenu.IsChecked = false;
            if (MyMediaElement.Stretch == Stretch.Fill)
                StretchVideoMenu.IsChecked = true;
            else
                StretchVideoMenu.IsChecked = false;
        }

        private void FitPlayerToVideoMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.FitPlayerToVideo == false)
                Properties.Settings.Default.FitPlayerToVideo = true;
        }

        private void FitPlayerToVideoMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.FitPlayerToVideo == true)
                Properties.Settings.Default.FitPlayerToVideo = false;
        }

        private void RepeatOneMenu_Click(object sender, RoutedEventArgs e)
        {
            this.RepeatState = Repeat_States.RepeatOne;
        }

        private void RepeatAllMenu_Click(object sender, RoutedEventArgs e)
        {
            this.RepeatState = Repeat_States.RepeatAll;
        }

        private void NoRepeatMenu_Click(object sender, RoutedEventArgs e)
        {
            this.RepeatState = Repeat_States.NoRepeat;
        }

        private void RepeatMenu_Loaded(object sender, RoutedEventArgs e)
        {
            NoRepeatMenu.IsChecked = RepeatOneMenu.IsChecked = RepeatAllMenu.IsChecked = false;
            switch (this.RepeatState)
            {
                case Repeat_States.NoRepeat:
                    NoRepeatMenu.IsChecked = true;
                    break;
                case Repeat_States.RepeatOne:
                    RepeatOneMenu.IsChecked = true;
                    break;
                case Repeat_States.RepeatAll:
                    RepeatAllMenu.IsChecked = true;
                    break;
            }
        }

        TimeSpan? A = null, B = null;

        private void SetRepeatABMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!A.HasValue && !B.HasValue)
            {
                A = MyMediaElement.Position;
            }
            else if (A.HasValue && !B.HasValue)
            {
                B = MyMediaElement.Position;
                SetPosition(A.Value);
            }
            else if (A.HasValue && B.HasValue)
            {
                A = B = null;
            }
        }

        private void RepeatABMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (!A.HasValue && !B.HasValue)
            {
                SetRepeatABMenu.Header = "Set A";
            }
            else if (A.HasValue && !B.HasValue)
            {
                SetRepeatABMenu.Header = "Set B";
            }
            else if (A.HasValue && B.HasValue)
            {
                SetRepeatABMenu.Header = "Cancel";
            }
        }

        private void PlaySpeedMenu_Loaded(object sender, RoutedEventArgs e)
        {
            PlayFastMenu.IsChecked = PlayNormalMenu.IsChecked = PlaySlowMenu.IsChecked = false;
            if (MyMediaElement.SpeedRatio == 2)
                PlayFastMenu.IsChecked = true;
            else if (MyMediaElement.SpeedRatio == 1)
                PlayNormalMenu.IsChecked = true;
            else if (MyMediaElement.SpeedRatio == .5)
                PlaySlowMenu.IsChecked = true;
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow AboutInstance = new AboutWindow();
            AboutInstance.Owner = this;
            AboutInstance.ShowDialog();
        }

        public void LoadSetting()
        {
            SetVolume(Properties.Settings.Default.PlayerVolume);
            if (!(Properties.Settings.Default.WindowWidth == 0 && Properties.Settings.Default.WindowHeight == 0))
            {
                this.Width = Properties.Settings.Default.WindowWidth;
                this.Height = Properties.Settings.Default.WindowHeight;
            }
            if (!(Properties.Settings.Default.WindowX == 0 && Properties.Settings.Default.WindowY == 0))
            {
                this.Left = Properties.Settings.Default.WindowX;
                this.Top = Properties.Settings.Default.WindowY;
            }
            this.RepeatState = (Repeat_States)Properties.Settings.Default.Repeat;
            this.MyMediaElement.Stretch = Properties.Settings.Default.StretchVideo ? Stretch.Fill : Stretch.Uniform;
        }
        public void SaveSetting()
        {
            Properties.Settings.Default.FitPlayerToVideo = FitPlayerToVideoMenu.IsChecked;
            Properties.Settings.Default.PlayerVolume = MyMediaElement.Volume;
            Properties.Settings.Default.WindowX = this.Left;
            Properties.Settings.Default.WindowY = this.Top;
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.Repeat = (int)this.RepeatState;
            Properties.Settings.Default.StretchVideo = this.MyMediaElement.Stretch == Stretch.Fill;
            Properties.Settings.Default.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSetting();
        }

        private void MakeSnapShotMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!MyMediaElement.HasVideo)
                return;
            string temp_file;
            try
            {
                temp_file = MakeSnapshot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error");
                return;
            }
            Microsoft.Win32.SaveFileDialog save_dialog = new Microsoft.Win32.SaveFileDialog();
            save_dialog.Filter = "PNG Image|*.png";
            bool? result = save_dialog.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                try
                {
                    System.IO.File.Move(temp_file, save_dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Error");
                }
            }
            else
            {
                try
                {
                    System.IO.File.Delete(temp_file);
                }
                catch { }
            }
        }

        private void StretchVideoMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StretchVideoMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (MyMediaElement.Stretch != Stretch.Fill)
                MyMediaElement.Stretch = Stretch.Fill;
        }

        private void StretchVideoMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            if (MyMediaElement.Stretch != Stretch.Uniform)
                MyMediaElement.Stretch = Stretch.Uniform;
        }

        public void FastForward()
        {
            MyMediaElement.SpeedRatio = 2;
        }

        public void ReWind()
        {
            MyMediaElement.SpeedRatio = .5;
        }

        private void PlayListMenu_Loaded(object sender, RoutedEventArgs e)
        {
            PlayListMenu.Items.Clear();
            foreach (Media media in this.PlayList)
            {
                MenuItem menu = new MenuItem();
                menu.Header = media.Name;
                menu.Tag = media;
                menu.Click += new RoutedEventHandler(PlayListSubMenu_Click);
                if (MyMediaElement.Source != null)
                {
                    Media current_media = this.PlayList[MyMediaElement.Source.LocalPath];
                    if (current_media != null && current_media == media)
                        menu.IsChecked = true;
                }
                PlayListMenu.Items.Add(menu);
            }
            if (PlayListMenu.Items.Count == 0)
                PlayListMenu.Visibility = Visibility.Collapsed;
            else
                PlayListMenu.Visibility = Visibility.Visible;
        }

        void PlayListSubMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
            {
                MenuItem item = (MenuItem)sender;
                if (item.Tag is Media)
                    Open((Media)item.Tag);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.FullScreen)
            {
                if (IsCursorHidden())
                {
                    ShowCursor();
                    if (MouseTimer.Enabled)
                        MouseTimer.Enabled = false;
                    MouseTimer.Start();
                }
                if (IsAtPanel(e.GetPosition(this).Y))
                {
                    if (mediaPanel.Visibility != Visibility.Visible)
                        mediaPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    if (mediaPanel.Visibility == Visibility.Visible)
                        mediaPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
        bool IsCursorHidden()
        {
            return Mouse.OverrideCursor == System.Windows.Input.Cursors.None;
        }
        public void ShowCursor()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                Mouse.OverrideCursor = null;
            }));
        }
        public void HideCursor()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.None;
            }));
        }

        int Default_Height = 110;

        bool IsAtPanel(double y)
        {
            return this.ActualHeight - y < Default_Height + 40;
        }

        private void MyMediaElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Properties.Settings.Default.PauseWhenClick && e.ChangedButton == MouseButton.Middle)
            {
                if (PlayState == Play_States.Playing)
                    Pause();
                else
                    Play();
            }
        }

        private void SetVolumeOnMouseWheelMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetVolumeOnMouseWheelMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SetVolumeOnScroll == false)
                Properties.Settings.Default.SetVolumeOnScroll = true;
        }

        private void SetVolumeOnMouseWheelMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SetVolumeOnScroll == true)
                Properties.Settings.Default.SetVolumeOnScroll = false;
        }

        private void SettingMenu_Loaded(object sender, RoutedEventArgs e)
        {
            SetVolumeOnMouseWheelMenu.IsChecked = Properties.Settings.Default.SetVolumeOnScroll;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(e.NewValue);
        }
        //bool MovedUp = false;
        //public void MoveUp()
        //{
        //    if (MovedUp)
        //        return;
        //    DoubleAnimation da = new DoubleAnimation();
        //    da.From = 0;
        //    da.To = defaultHeight;
        //    da.Duration = new Duration(TimeSpan.FromSeconds(0.2));
        //    da.FillBehavior = FillBehavior.Stop;

        //    mediaPanel.BeginAnimation(Grid.HeightProperty, da);
        //    MovedUp = true;
        //}

        //public void MoveDown()
        //{
        //    if (mediaPanel.Height != defaultHeight)
        //        return;
        //    DoubleAnimation da = new DoubleAnimation();
        //    da.From = defaultHeight;
        //    da.To = 0;
        //    da.Duration = new Duration(TimeSpan.FromSeconds(0.2));
        //    da.FillBehavior = FillBehavior.Stop;

        //    //da.Completed += (object sender, EventArgs args) =>
        //    //{
        //    //    mediaPanel.Visibility = Visibility.Hidden;
        //    //};
        //    mediaPanel.BeginAnimation(Grid.HeightProperty, da);
        //    MovedUp = false;
        //}

    }
}
