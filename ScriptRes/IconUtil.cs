using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ScriptRes
{
    internal class IconUtil
    {
        public System.Windows.Controls.Image ImageControl { get; set; }
        private Icon? CurrentIcon { get; set; }
        public string SavedIconPath { get; private set; } = String.Empty;

        public IconUtil(System.Windows.Controls.Image control) 
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

        public static Icon[] ExtractAllIcons(string filePath, uint MaxIcons = 10)
        {
            if (File.Exists(filePath) == false)
            {
                throw new Exception($"The file: {filePath} doesn't exist or moved.");
            }

            Icon[] icons;

            if (VerifyMultiIconExtension(filePath))
            {
                // Get the amount of icons stored
                uint iconsCount = ExtractIconEx(filePath, -1, null, null, 1);
                if (iconsCount == 0)
                    return Array.Empty<Icon>();  // No icons found
                if (iconsCount > MaxIcons)
                    iconsCount = MaxIcons;

                icons = new Icon[iconsCount];
                IntPtr[] largeIcons = new IntPtr[iconsCount];
                IntPtr[] smallIcons = new IntPtr[iconsCount];
                _ = ExtractIconEx(filePath, 0, largeIcons, smallIcons, iconsCount);

                for (int i = 0; i < iconsCount; i++)
                {
                    if (largeIcons[i] != IntPtr.Zero)
                    {
                        icons[i] = (Icon)Icon.FromHandle(largeIcons[i]).Clone();
                        _ = DestroyIcon(largeIcons[i]);
                    }
                    else if (smallIcons[i] != IntPtr.Zero)
                    {
                        icons[i] = (Icon)Icon.FromHandle(smallIcons[i]).Clone();
                        _ = DestroyIcon(smallIcons[i]);
                    }
                }
            }
            else
            {
                //Icon? icon = Icon.ExtractAssociatedIcon(filePath);
                //icons = icon == null ? Array.Empty<Icon>() : new Icon[1] { icon };
                icons = Array.Empty<Icon>();
            }

            return icons;
        }

        private static bool VerifyMultiIconExtension(string filePath)
        {
            return Path.GetExtension(filePath) switch
            {
                ".exe" => true,
                ".dll" => true,
                ".ico" => true,
                _ => false,
            };
        }


        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string lpszFile, int nIconIndex, [Out] IntPtr[]? phiconLarge, [Out] IntPtr[]? phiconSmall, [In] uint nIcons);

        [DllImport("user32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
    }
}
