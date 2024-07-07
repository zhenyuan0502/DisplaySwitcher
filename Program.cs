using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinRT;

namespace DisplaySwitcher
{
    public class Program
    {
        [STAThread]
        static async Task<int> Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            bool isRedirect = await DecideRedirection();
            if (!isRedirect)
            {
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }
            return 0;
        }

        private static async Task<bool> DecideRedirection()
        {
            bool isRedirect = false;
            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
            ExtendedActivationKind kind = args.Kind;
            AppInstance keyInstance = AppInstance.FindOrRegisterForKey("ARBITRARY-KEY-THAT-IDENTIFIES-YOUR-APP");

            if (keyInstance.IsCurrent)
            {
                keyInstance.Activated += OnActivated;
            }
            else
            {
                isRedirect = true;
                await keyInstance.RedirectActivationToAsync(args);
            }
            return isRedirect;
        }

        //// Do the redirection on another thread, and use a non-blocking
        //// wait method to wait for the redirection to complete.
        //public static void RedirectActivationTo(
        //    AppActivationArguments args, AppInstance keyInstance)
        //{
        //    var redirectSemaphore = new Semaphore(0, 1);
        //    Task.Run(() =>
        //    {
        //        keyInstance.RedirectActivationToAsync(args).AsTask().Wait();
        //        redirectSemaphore.Release();
        //    });
        //    redirectSemaphore.WaitOne();
        //}

        private static void OnActivated(object sender, AppActivationArguments args)
        {
            ExtendedActivationKind kind = args.Kind;

            var e = args.Data as Windows.ApplicationModel.Activation.LaunchActivatedEventArgs;
            DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION[e.Arguments]);
        }
    }
}
