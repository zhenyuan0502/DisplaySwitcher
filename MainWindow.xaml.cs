using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
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
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Shell;
using Windows.UI.StartScreen;
using WinRT.Interop;
using System.Runtime.InteropServices; // For DllImport
using WinRT;
using DisplaySwitcher.Helper; // required to support Window.As<ICompositionSupportsSystemBackdrop>()

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DisplaySwitcher
{
    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static unsafe extern int CreateDispatcherQueueController(DispatcherQueueOptions options, IntPtr* instance);

        IntPtr m_dispatcherQueueController = IntPtr.Zero;
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (m_dispatcherQueueController == IntPtr.Zero)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                unsafe
                {
                    IntPtr dispatcherQueueController;
                    CreateDispatcherQueueController(options, &dispatcherQueueController);
                    m_dispatcherQueueController = dispatcherQueueController;
                }
            }
        }
    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See below for implementation.
        MicaController m_backdropController;
        SystemBackdropConfiguration m_configurationSource;
        AppWindow m_appWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                m_appWindow = WindowHelper.GetAppWindow(this);
                m_appWindow.SetIcon(@"Assets\TitlebarLogo.ico");
                m_appWindow.Resize(new(480, 280));

                //https://learn.microsoft.com/en-us/windows/apps/develop/title-bar
                var titleBar = m_appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
            }

            TrySetSystemBackdrop();
        }


        private void SetCapitionButtonColorForWin11()
        {
            //https://learn.microsoft.com/en-us/windows/apps/develop/title-bar#color-and-transparency-in-caption-buttons
            //titleBar.ButtonBackgroundColor = Colors.Transparent;
            //titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var currentTheme = ((FrameworkElement)Content).ActualTheme;
            if (currentTheme == ElementTheme.Dark)
            {
                m_appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                m_appWindow.TitleBar.ButtonForegroundColor = Colors.White;
                m_appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            }
            else if (currentTheme == ElementTheme.Light)
            {
                m_appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                m_appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                m_appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            }
        }

        bool TrySetSystemBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Create the policy object.
                m_configurationSource = new SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_backdropController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_backdropController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_backdropController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed
            // so it doesn't try to use this closed window.
            if (m_backdropController != null)
            {
                m_backdropController.Dispose();
                m_backdropController = null;
            }
            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }

            SetCapitionButtonColorForWin11();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
                (this.Content as FrameworkElement).RequestedTheme = toggleSwitch.IsOn ? ElementTheme.Dark : ElementTheme.Light;

            ApplicationData.Current.LocalSettings.Values["isDarkTheme"] = ((ToggleSwitch)sender).IsOn;
        }

        private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("isDarkTheme", out object themeSetting))
            {
                darkSwitch.IsOn = (bool)themeSetting;
                (this.Content as FrameworkElement).RequestedTheme = (bool)themeSetting ? ElementTheme.Dark : ElementTheme.Light;
            }
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
