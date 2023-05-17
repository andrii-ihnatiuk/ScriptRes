using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ScriptRes
{
    internal class IconUtil
    {
        public Image ImageControl { get; set; }
        private System.Drawing.Icon? CurrentIcon { get; set; }
        public string SavedIconPath { get; private set; } = String.Empty;

        public IconUtil(Image control) 
        { 
            ImageControl = control;
        }

        public void DisplayIcon(string iconPath)
        {
            CurrentIcon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);

            if (CurrentIcon != null)
            {
                // Convert the icon to a BitmapSource
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    CurrentIcon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());


                ImageControl.Width = bitmapSource.Width;
                ImageControl.Height = bitmapSource.Height;
                ImageControl.Source = bitmapSource;
            }
        }

        public bool SaveIcon(string saveLocation)
        {
            if (CurrentIcon != null)
            {
                if (Directory.Exists(saveLocation))
                {
                    string filePath = Path.Combine(saveLocation, Guid.NewGuid().ToString() + ".ico");

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    CurrentIcon.Save(stream);

                    SavedIconPath = filePath;
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
