using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DisplaySwitcher
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        public string LaunchParam { get; set; }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            LaunchParam = args.Arguments;
            // Show message log in the console
            Console.WriteLine($"LaunchParam: {LaunchParam}");

            m_window = new MainWindow();
            m_window.Activate();
            m_window.AppWindow.Resize(new (480, 800));
            m_window.AppWindow.SetIcon("Assets/TitlebarLogo.png");

            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();
            await jumpList.SaveAsync();

            var list = new List<string>() { "First", "Second", "Duplicate", "Extend" };
            bool isChanged = false;
            foreach (var item in list)
            {
                if (jumpList.Items.Any(x => x.Arguments == item))
                    continue;

                var jumpListItem = JumpListItem.CreateWithArguments(item, item);
                jumpListItem.Description = "ms-resource:///Resources/CustomJumpListItemDescription";
                jumpListItem.GroupName = "";
                jumpListItem.Logo = new Uri("ms-appx:///Assets/TitlebarLogo.png");
                jumpList.Items.Add(jumpListItem);
                isChanged = true;
            }

            if (isChanged)
                await jumpList.SaveAsync();
        }

        private Window m_window;
    }
}
