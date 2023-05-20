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
        private string _iconsSource = string.Empty;

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

        // Handler for buttons to select file locations
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;

            if (btn == btnChangeQresPath)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Executable files (*.exe)|*.exe" };
                if (openFileDialog.ShowDialog() == true)
                {
                    tBoxQresPath.Text = openFileDialog.FileName;
                }
            }
            else if (btn == btnBrowseExec || btn == btnCustomIcon)
            {
                string filter = btn == btnBrowseExec ? "All files (*.*)|*.*" : "Files with icons (*.ico;*.exe;*.dll)|*.ico;*.exe;*.dll";
                OpenFileDialog openFileDialog = new OpenFileDialog { Filter = filter };
                if (openFileDialog.ShowDialog() == true)
                {
                    _iconsSource = openFileDialog.FileName;
                    if (btn == btnBrowseExec)
                    {
                        tBoxExecPath.Text = openFileDialog.FileName;
                    }
                    // Get all icons associated with a file
                    System.Drawing.Icon[] extractedIcons;
                    try
                    {
                        extractedIcons = IconUtil.ExtractAllIcons(_iconsSource);
                    }
                    catch (FileNotFoundException exc) 
                    {
                        MessageBox.Show($"An error occured while extracting icons from the file.\nDetails: {exc.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
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
                    listBoxIcons.SelectedIndex = 0; // first element is auto-selected

                    tBoxExtractedIcons.Text = itemsSource.Count > 0 ? "Select the icon" : "Couldn't extract icons";
                    tBoxShortcutName.IsEnabled = true;
                    if (btn == btnBrowseExec)
                    {
                        tBoxShortcutName.Text = Path.GetFileNameWithoutExtension(_iconsSource);
                    }
                    btnCustomIcon.Visibility = Visibility.Visible;
                }
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

            string script = $"@echo off\r\n" +
                            "chcp 65001\r\n\r\n" +
                            "set exitCode=0\r\n\r\n" +
                            $"echo Changing the resolution to {outX}x{outY} ...\r\n" +
                            $"\"{qresPath}\" /x:{outX} /y:{outY} | findstr /i \"Error:\" > nul\r\n" +
                            "if %errorlevel% equ 0 (\r\n" +
                            "    set exitCode=-1\r\n" +
                            "    echo ERROR: Something went wrong, it is likely that the resolution is not supported.\r\n" +
                            ")\r\n\r\n" +
                            "echo.\r\n" +
                            $"echo Launching the program \"{execName}\"\r\n" +
                            $"start /b /wait \"\" \"{execPath}\"\r\n\r\n" +
                            "echo.\r\n" +
                            "echo Changing resolution back to normal\r\n" +
                            $"\"{qresPath}\" /x:{inX} /y:{inY} | findstr /i \"Error:\" > nul\r\n" +
                            "if %errorlevel% equ 0 (\r\n" +
                            "    set exitCode=-1\r\n" +
                            "    echo ERROR: Something went wrong, it is likely that the resolution is not supported.\r\n" +
                            ")\r\n\r\n" +
                            "echo.\r\n" +
                            "echo Finish\r\n" +
                            "timeout /t 2 > nul 2>&1\r\n" +
                            "exit /b %exitCode%";
            try
            {
                string scriptsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
                Directory.CreateDirectory(scriptsLocation);

                string batchLocation = Path.Combine(scriptsLocation, $"{execNameNoExt}.bat");
                File.WriteAllText(batchLocation, script);

                string shortcutName = string.IsNullOrWhiteSpace(tBoxShortcutName.Text) ? execNameNoExt : tBoxShortcutName.Text;
               
                CreateShortcut(desktopPath, batchLocation, _iconsSource, shortcutName);
                MessageBox.Show($"The shortcut was created: \n{desktopPath}\\{shortcutName}.lnk", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            tBoxQresPath.Text = "./QRes.exe";
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
