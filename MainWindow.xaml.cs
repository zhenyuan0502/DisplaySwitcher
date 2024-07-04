using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.StartScreen;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DisplaySwitcher
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private string m_windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        private string m_displaySwitchPath = string.Empty;
        public MainWindow()
        {
            this.InitializeComponent();
            m_displaySwitchPath = Path.Combine(m_windowsPath, "System32", "DisplaySwitch");
        }

        private async void PinTaskBar_Click(object sender, RoutedEventArgs e)
        {
            //pinTaskBar.IsEnabled = false;
            //var isPinned = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();

            //// Update the UI to the appropriate state based on the results of the pin request.
            //pinTaskBar.IsEnabled = !isPinned;
            //pinTaskBar.IsEnabled = true;

            // Get the app's jump list.
            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();

            // Set the system to autogenerate a Frequent group for the app jump list.
            // Alternatively, this property could be set to JumpListSystemGroupKind.Recent to autogenerate a Recent group.
            jumpList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.Frequent;

            // No changes were made to the jump list Items property, so any custom tasks and groups remain intact.
            await jumpList.SaveAsync();
        }

        private void StartScreen(int number)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {m_displaySwitchPath} {number}";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            process.Close();
        }

        private void FirstScreen_Click(object sender, RoutedEventArgs e)
        {
            StartScreen(1);
        }

        private void SecondScreen_Click(object sender, RoutedEventArgs e)
        {
            StartScreen(4);
        }

        private void DuplicateScreen_Click(object sender, RoutedEventArgs e)
        {
            StartScreen(2);
        }

        private void ExtendScreen_Click(object sender, RoutedEventArgs e)
        {
            StartScreen(3);
        }

    }
}
