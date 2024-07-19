using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;

namespace DisplaySwitcher.Helper
{
    public static class UIHelper
    {
        static public void AnnounceActionForAccessibility(UIElement ue, string annoucement, string activityID)
        {
            var peer = FrameworkElementAutomationPeer.FromElement(ue);
            peer.RaiseNotificationEvent(AutomationNotificationKind.ActionCompleted,
                                        AutomationNotificationProcessing.ImportantMostRecent, annoucement, activityID);
        }
    }
}
