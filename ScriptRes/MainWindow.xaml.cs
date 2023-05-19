using Microsoft.Win32;
using ScriptRes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ScriptRes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chBoxQresPath.IsChecked = true;
            chBoxQresPath.Checked += CheckBox_Checked;
            chBoxQresPath.Unchecked += CheckBox_Checked;
        }

        // Handler for buttons which select file locations
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;

            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "All Files (*.*)|*.*" };

            if (btn == btnChangeQresPath && openFileDialog.ShowDialog() == true)
            {
                tBoxQresPath.Text = openFileDialog.FileName;
            }
            else if (btn == btnBrowseExec && openFileDialog.ShowDialog() == true)
            {
                // In case the file was deleted or moved after selection
                if (File.Exists(openFileDialog.FileName) == false)
                {
                    MessageBox.Show("Provided program executable doesn't exist or not a file!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                tBoxExecPath.Text = openFileDialog.FileName;

                // Get all icons associated with a file
                var extractedIcons = IconUtil.ExtractAllIcons(openFileDialog.FileName);
                // Display all icons in ListBox

                var itemsSource = new List<IconListItem>();
                for (int i = 0; i < extractedIcons.Length; i++)
                {
                    var imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        extractedIcons[i].Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    itemsSource.Add(new IconListItem(i, imageSource));
                }
                listBoxIcons.ItemsSource = itemsSource;
                tBoxExtractedIcons.Text = itemsSource.Count > 0 ? "Select the icon" : "Couldn't extract icons";
            }
        }

        // Handler for script generation button
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string inX = tBoxInX.Text;
            string inY = tBoxInY.Text;
            string outX = tBoxOutX.Text;
            string outY = tBoxOutY.Text;
            string execPath = tBoxExecPath.Text;
            string qresPath = tBoxQresPath.Text;

            if (string.IsNullOrEmpty(inX) || string.IsNullOrEmpty(inY) || string.IsNullOrEmpty(outX) || string.IsNullOrEmpty(outY) || string.IsNullOrEmpty(qresPath))
            {
                return;
            }

            if (File.Exists(execPath) == false)
            {
                MessageBox.Show("Provided program executable doesn't exist or not a file!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (File.Exists(qresPath) && Path.GetFileName(qresPath) == "QRes.exe")
            {
                // Allow using relative path in qres location textbox
                if (Path.IsPathFullyQualified(qresPath) == false)
                {
                    qresPath = Path.GetFullPath(qresPath);
                }
            }
            else
            {
                MessageBox.Show("Provided QRes.exe location doesn't exist or the file is wrong!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string execName = Path.GetFileName(execPath);
            string execNameNoExt = Path.GetFileNameWithoutExtension(execPath);

            string script = $"@echo off\r\n\r\n" +
                            "set exitCode=0\r\n" +
                            $"echo Changing the resolution to {outX}x{outY} ...\r\n" +
                            $"\"{qresPath}\" /x:{outX} /y:{outY} | findstr /i \"Error:\" > nul\r\n" +
                            "if %errorlevel% equ 0 (\r\n" +
                            "    set exitCode=-1\r\n" +
                            "    goto error\r\n" +
                            ")\r\n" +
                            "echo.\r\n" +
                            $"echo Launching the program \"{execName}\"\r\n" +
                            "echo.\r\n" +
                            $"\"{execPath}\"\r\n\r\n" +
                            $":loop\r\ntasklist | findstr /i \"{execName}\" > nul\r\n" +
                            "if not %errorlevel% equ 0 (\r\n" +
                            "    echo Changing resolution back to normal\r\n" +
                            $"    \"{qresPath}\" /x:{inX} /y:{inY} > nul\r\n" +
                             "    goto end\r\n) \r\n\r\n" +
                            "timeout /t 2 > nul 2>&1\r\n" +
                            "goto loop\r\n\r\n" +
                            ":error\r\n" +
                            "echo ERROR: Something went wrong, it is likely that the resolution is not supported.\r\n\r\n" +
                            ":end\r\n" +
                            "echo.\r\n" +
                            "echo Finish\r\n" +
                            "timeout /t 2 > nul 2>&1\r\n" +
                            "exit /b %exitCode%";
            try
            {
                string batchLocation = Path.Combine(desktopPath, $"{execNameNoExt}.bat");
                File.WriteAllText(batchLocation, script);
                string saveLocation = Environment.CurrentDirectory;
               
                CreateShortcut(desktopPath, batchLocation, execPath, execNameNoExt);
                MessageBox.Show($"The shortcut was created: \n{desktopPath}\\{execNameNoExt}.lnk", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckBox_Checked(object sender, EventArgs e) 
        {
            if (sender is not CheckBox box) return;

            tBoxQresPath.IsEnabled = !box.IsChecked ?? false;
            tBoxQresPath.Text = "./Qres.exe";
            btnChangeQresPath.IsEnabled = !box.IsChecked ?? false;
        }

        private void CreateShortcut(string shortcutPath, string targetPath, string? iconPath, string name)
        {
            var shell = new IWshRuntimeLibrary.WshShell();
            string location = Path.Combine(shortcutPath, $"{name}.lnk");
            var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(location);

            if (listBoxIcons.SelectedItem is IconListItem selectedIcon)
            {
                iconPath += $",{selectedIcon.Id}";
            }

            shortcut.TargetPath = targetPath;
            shortcut.Description = "This shortcut was generated by ScriptRes";
            if (string.IsNullOrEmpty(iconPath) == false)
            {
                shortcut.IconLocation = iconPath;
            }
            shortcut.Save();
        }
    }
}
