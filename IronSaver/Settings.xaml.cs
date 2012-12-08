using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;

namespace IronSaver
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>

    public partial class Settings : System.Windows.Window
    {
        public static string[] ValidExtensions {
            get
            {
                return new string[] { ".png", ".jpg", ".jpeg", ".bmp" };
            }
        }
        public static string PictureFolder {
            set 
            {
                IronSaver.Properties.Settings.Default.PictureFolder = value;
            }
            get
            {
                string path = IronSaver.Properties.Settings.Default.PictureFolder;
                if("%Pictures%".Equals(path)) {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }
                return path;
            }
        }
        public static TimeSpan ChangeInterval {
            get
            {
                return IronSaver.Properties.Settings.Default.ChangeInterval;
            }
            set
            {
                IronSaver.Properties.Settings.Default.ChangeInterval = value;
            }
        }
        public static bool ShowClock {
            get
            {
                return IronSaver.Properties.Settings.Default.ShowClock;
            }
            set
            {
                IronSaver.Properties.Settings.Default.ShowClock = value;
            }
        }

        private void SaveSettings()
        {
            IronSaver.Properties.Settings.Default.Save();
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void browse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the folder where to use the pictures from.";
            dialog.SelectedPath = Settings.PictureFolder;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(dialog.SelectedPath))
                {
                    var files = (from file in new DirectoryInfo(dialog.SelectedPath).GetFiles()
                                 where Settings.ValidExtensions.Any(ext => ext.Equals(file.Extension))
                                 select file);
                    if (files.Count() == 0)
                    {
                        System.Windows.MessageBox.Show(this, "The folder you have selected does not have any valid pictures. Only JPG, PNG and BMP files can be used with this screensaver");
                    }
                    else
                    {
                        Settings.PictureFolder = dialog.SelectedPath;
                        picturePath.Content = Settings.PictureFolder; //TODO: should be done with databinding
                    }
                }
            }
        }
    }
}