using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DisplaySwitcher.Views;

[ObservableObject]
public sealed partial class TrayIconView : UserControl
{
    [ObservableProperty]
    private bool _isWindowVisible;
    
    public TrayIconView()
    {
        InitializeComponent();
    }

    [RelayCommand]
    public void ShowHideWindow()
    {
        Window window = App.MainWindow;
        if (window == null)
        {
            return;
        }

        if (window.Visible)
        {
            window.Hide();
        }
        else
        {
            window.Show();
        }
        IsWindowVisible = window.Visible;
    }

    [RelayCommand]
    public void ExitApplication()
    {
        App.HandleClosedEvents = false;
        TrayIcon.Dispose();
        App.MainWindow?.Close();
    }

    private void First_Click(object sender, RoutedEventArgs e) 
        => DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["First"]);

    private void Second_Click(object sender, RoutedEventArgs e)
        => DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["Second"]);

    private void Duplicate_Click(object sender, RoutedEventArgs e)
        => DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["Duplicate"]);
    private void Extend_Click(object sender, RoutedEventArgs e)
        => DisplaySwitcherFunction.StartScreen(DisplaySwitcherFunction.SCREEN_OPTION["Extend"]);
}
