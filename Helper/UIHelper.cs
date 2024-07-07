using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
