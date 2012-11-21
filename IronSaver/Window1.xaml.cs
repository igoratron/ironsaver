using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace IronSaver
{
    /// <summary>
    /// Interaction logic for Window1
    /// </summary>

    public partial class SaverWindow : System.Windows.Window
    {
        private string[] validExtensions = new string[] { ".png", ".jpg", ".bmp" };
        private string pictureFolder = @"C:\Users\Igor\Pictures\hotrod";
        private TimeSpan interval = new TimeSpan(0, 0, 10);
        private CachedBitmap[] pictures = null;
        private Random random = new Random();

        public SaverWindow()
        {
            InitializeComponent();
        }

        void OnLoaded(object sender, EventArgs e)
        {
#if !DEBUG
            Topmost = true;
            MouseMove += new MouseEventHandler(PhotoStack_MouseMove);
            MouseDown += new MouseButtonEventHandler(PhotoStack_MouseDown);
            KeyDown += new KeyEventHandler(PhotoStack_KeyDown);
#endif
            //find all valid images
            var files = (from file in new DirectoryInfo(pictureFolder).GetFiles()
                       where validExtensions.Any(ext => ext.Equals(file.Extension))
                       select file).ToArray();
            pictures = new CachedBitmap[files.Count()];
            for (int i = 0; i < pictures.Count(); i++)
            {
                pictures[i] = new CachedBitmap(new BitmapImage(new Uri(files[i].FullName)), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            //todo: shuffle array

            ////load first imag
            firstImage.Source = pictures[random.Next(pictures.Count())];

            //fade in
            Storyboard fadeIn = (Storyboard)FindResource("fadeIn");
            Dispatcher.BeginInvoke(new Action(fadeIn.Begin), DispatcherPriority.Normal);

            ////start timer to switch pictures
            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(ChangePhotos);
            dispatcherTimer.Interval = interval;
            dispatcherTimer.Start();
        }

        void ChangePhotos(object sender, EventArgs e)
        {
            int current = random.Next(pictures.Count());
            secondImage.Source = pictures[current];

            Storyboard transition = (Storyboard)secondImage.FindResource("transition");
            transition.Completed += transition_Completed;
            transition.Begin();
        }

        void transition_Completed(object sender, EventArgs e)
        {
            firstImage.Source = secondImage.Source;
        }

        void PhotoStack_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void PhotoStack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void PhotoStack_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.MouseDevice.GetPosition(this);
            // Set IsActive and MouseLocation only the first time this event is called.
            if (!isActive)
            {
                mousePosition = currentPosition;
                isActive = true;
            }
            else
            {
                // If the mouse has moved significantly since first call, close.
                if ((Math.Abs(mousePosition.X - currentPosition.X) > 10) ||
                    (Math.Abs(mousePosition.Y - currentPosition.Y) > 10))
                {
                    Application.Current.Shutdown();
                }
            }
        }

        bool isActive;
        Point mousePosition;
    }
}