using H.NotifyIcon;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
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

        public static MainWindow MainWindow { get; set; } = new();
        public static bool HandleClosedEvents { get; set; } = true;


        private async Task CreateJumpList()
        {
            var jumpList = await JumpList.LoadCurrentAsync();
            //jumpList.Items.Clear();
            //await jumpList.SaveAsync();

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

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
        {
            if (e.UWPLaunchActivatedEventArgs.PreviousExecutionState != ApplicationExecutionState.Running) //Check if is there any instance of the App is already running
                base.OnLaunched(e);

            MainWindow.Closed += (sender, args) =>
            {
                if (HandleClosedEvents)
                {
                    args.Handled = true;
                    MainWindow.Hide();
                }
            };

            MainWindow.Activate();

            await CreateJumpList();
        }


        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }
    }
}
