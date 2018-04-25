using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace AeroPlayer
{
    static public class Snapshoter
    {
        /// <summary>
        /// Need for open media
        /// </summary>
        static MediaPlayer mediaPlayer = new MediaPlayer();

        /// <summary>
        /// Path to snapshot
        /// </summary>
        static string photoPath;

        /// <summary>
        /// can't run while not finished
        /// </summary>
        static bool running = false;

        /// <summary>
        /// Make snapshot
        /// </summary>
        /// <param name="position">position of snapshot</param>
        /// <param name="fileName">source</param>
        /// <param name="photoPath">snapshot path</param>
        static public void CaptureFrame(TimeSpan position, string fileName, string photoPath)
        {
            if (running)
                return;
            running = true;
            Snapshoter.photoPath = photoPath;
            //open media
            mediaPlayer.Open(new Uri(fileName));
            //turn off the sound
            mediaPlayer.Volume = 0;
            //mediaPlayer.Play();
            mediaPlayer.MediaOpened += new EventHandler(mediaPlayer_MediaOpened);

            mediaPlayer.Position = position;
            mediaPlayer.ScrubbingEnabled = true;
        }

        /// <summary>
        /// when media is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void mediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            DrawingVisual visual = new DrawingVisual();

            int naturalWidth = mediaPlayer.NaturalVideoWidth;
            int naturalHeight = mediaPlayer.NaturalVideoHeight;

            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawVideo(mediaPlayer, new Rect(0, 0, naturalWidth, naturalHeight));
                //dc.DrawLine(new Pen(Brushes.AntiqueWhite, 2), new Point(0, 0), new Point(100, 100));
            }

            Size desiredSize = new Size(naturalWidth, naturalHeight);
            Size dpi = new Size(96, 96);
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)desiredSize.Width,
            (int)desiredSize.Height, dpi.Width, dpi.Height, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            FileStream stream = new FileStream(photoPath, FileMode.Create);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.On;
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            stream.Flush();
            stream.Dispose();
            mediaPlayer.Close();
            running = false;
        }
    }
}
