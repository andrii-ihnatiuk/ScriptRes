using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*"
            };
            if (btn == btnChangeQresPath && openFileDialog.ShowDialog() == true)
            {
                tBoxQresPath.Text = openFileDialog.FileName;
            }
            if (btn == btnBrowseExec && openFileDialog.ShowDialog() == true)
            {
                tBoxPath.Text = openFileDialog.FileName;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string inX = tBoxInX.Text;
            string inY = tBoxInY.Text;
            string outX = tBoxOutX.Text;
            string outY = tBoxOutY.Text;
            string execPath = tBoxPath.Text;
            string qresPath = tBoxQresPath.Text;

            if (string.IsNullOrEmpty(inX) || string.IsNullOrEmpty(inY) || string.IsNullOrEmpty(outX) || string.IsNullOrEmpty(outY) || string.IsNullOrEmpty(qresPath))
            {
                return;
            }

            if (!System.IO.Path.Exists(execPath) || !System.IO.Path.IsPathFullyQualified(execPath))
            {
                return;
            }
           
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string precessName = System.IO.Path.GetFileName(execPath);
            string noExtName = System.IO.Path.GetFileNameWithoutExtension(execPath);

            string script = $"@echo off\r\n" +
                            $"set processName={precessName}\r\n\r\n" +
                            $"set inX={inX}\r\n" +
                            $"set inY={inY}\r\n\r\n" +
                            $"set outX={outX}\r\n" +
                            $"set outY={outY}\r\n\r\n" +
                            "echo Changing the resolution to %outX%x%outY% ...\r\n\"" +
                            "D:\\QRes\\QRes.exe\" /x:%outX% /y:%outY% > nul\r\n" +
                            "if not %errorlevel% equ 0 (\r\n" +
                            "    goto error\r\n" +
                            ")\r\n" +
                            "echo.\r\n" +
                            "echo Launching the program \"%processName%\"\r\n" +
                            "echo.\r\n" +
                            $"{tBoxPath.Text}\r\n\r\n" +
                            ":loop\r\ntasklist | findstr /i \"%processName%\" > nul\r\n" +
                            "if not %errorlevel% equ 0 (\r\n" +
                            "    echo Changing resolution back to normal\r\n" +
                            "    \"D:\\QRes\\QRes.exe\" /x:%inX% /y:%inY% > nul\r\n" +
                            "    goto end\r\n) \r\n\r\n" +
                            "timeout /t 2 > nul 2>&1\r\n" +
                            "goto loop\r\n\r\n" +
                            ":error\r\n" +
                            "echo !!!! Error occured while changing resolution !!!!\r\n\r\n" +
                            ":end\r\n" +
                            "echo.\r\n" +
                            "echo Bye\r\n" +
                            "timeout /t 2 > nul 2>&1";
            try
            {
                File.WriteAllText(desktopPath + $"\\{noExtName}.bat", script);
                MessageBox.Show($"The file was created: \n{desktopPath}\\{noExtName}.bat");
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
                
        }

        private void CheckBox_Checked(object sender, EventArgs e) 
        {
            if (sender is not CheckBox box) return;

            tBoxQresPath.IsEnabled = !box.IsChecked ?? false;
            tBoxQresPath.Text = "./Qres.exe";
            btnChangeQresPath.IsEnabled = !box.IsChecked ?? false;
        }
    }
}
