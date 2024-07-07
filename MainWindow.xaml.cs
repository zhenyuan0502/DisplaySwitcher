using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DisplaySwitcher
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        AppWindow m_appWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                m_appWindow = GetAppWindowForCurrentWindow();
                m_appWindow.SetIcon(@"Assets\TitlebarLogo.ico");
                m_appWindow.Resize(new(480, 320));

                //https://learn.microsoft.com/en-us/windows/apps/develop/title-bar
                var titleBar = m_appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                titleBarTextBlock.Text = "Display Switcher Extension";
            }
        }

        private void SwitchPresenter_CompOverlay(object sender, RoutedEventArgs e)
        {
            m_appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
        }

        private void SwitchPresenter_OverLapped(object sender, RoutedEventArgs e)
        {
            m_appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
        }

        private void SwitchPresenter_Fullscreen(object sender, RoutedEventArgs e)
        {
            m_appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(myWndId);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn == true)
                {
                    if (this.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = ElementTheme.Dark;
                    }
                }
                else
                {
                    if (this.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = ElementTheme.Light;
                    }
                }
            }

            ApplicationData.Current.LocalSettings.Values["themeSetting"] = ((ToggleSwitch)sender).IsOn ? 0 : 1;
        }

        private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("themeSetting", out object themeSetting) && (int)themeSetting == 0)
            {
                darkSwitch.IsOn = true;
            }
            else
            {
                darkSwitch.IsOn = false;
            }
        }

        private async void PinTaskBar_Click(object sender, RoutedEventArgs e)
        {
            //pinTaskBar.IsEnabled = false;
            //var isPinned = await TaskbarManager.GetDefault().RequestPinCurrentAppAsync();

            //// Update the UI to the appropriate state based on the results of the pin request.
            //pinTaskBar.IsEnabled = !isPinned;
            //pinTaskBar.IsEnabled = true;

        }

        private void FirstScreen_Click(object sender, RoutedEventArgs e)
        {
            DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["First"]);
        }

        private void SecondScreen_Click(object sender, RoutedEventArgs e)
        {
            DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["Second"]);
        }

        private void DuplicateScreen_Click(object sender, RoutedEventArgs e)
        {
            DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["Duplicate"]);
        }

        private void ExtendScreen_Click(object sender, RoutedEventArgs e)
        {
            DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["Extend"]);
        }

    }
}
