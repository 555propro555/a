using Android.App;
using Android.AccessibilityServices;
using Android.Content;
using Android.Views.Accessibility;
using Android.OS;
using Android.Views;

namespace GestureModifierApp
{
    // 1. Android 13 Boot Receiver: Automatically fires when phone powers up
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    [IntentFilter(new[] { 
        Intent.ActionBootCompleted, 
        Intent.ActionQuickBootPoweron, 
        "android.intent.action.QUICKBOOT_POWERON" 
    })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null) return;
            
            var launchIntent = context.PackageManager?.GetLaunchIntentForPackage(context.PackageName ?? "");
            if (launchIntent != null)
            {
                launchIntent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(launchIntent);
            }
        }
    }

    // 2. Android 13 Accessibility Service: Intercepts custom edge-swipes/gestures
    [Service(Permission = "android.permission.BIND_ACCESSIBILITY_SERVICE", Exported = false)]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice", Resource = "@xml/accessibility_service_config")]
    public class GestureAccessibilityService : AccessibilityService
    {
        private static GestureAccessibilityService? _instance;
        public static GestureAccessibilityService? Instance => _instance;

        public override void OnServiceConnected()
        {
            base.OnServiceConnected();
            _instance = this;
            
            var info = new AccessibilityServiceInfo
            {
                EventTypes = EventTypes.AllMask,
                FeedbackType = AccessibilityServiceFeedbackType.Generic,
                Flags = AccessibilityServiceFlags.Default | 
                        AccessibilityServiceFlags.RequestTouchExplorationMode | 
                        AccessibilityServiceFlags.RetrieveWindowContent,
                NotificationTimeout = 100
            };
            SetServiceInfo(info);
        }

        public override void OnAccessibilityEvent(AccessibilityEvent? e)
        {
            // Monitor system events for your gesture processing logic
        }

        public override void OnInterrupt()
        {
            _instance = null;
        }

        // --- System Gesture Action Handlers ---

        public void PerformBackAction()
        {
            PerformGlobalAction(GlobalAction.Back);
        }

        public void PerformHomeAction()
        {
            PerformGlobalAction(GlobalAction.Home);
        }

        public void PerformRecentAppsAction()
        {
            PerformGlobalAction(GlobalAction.Recents);
        }

        public void PerformLockScreenAction()
        {
            // Double Tap to Sleep (Android 9+)
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.P)
            {
                PerformGlobalAction(GlobalAction.LockScreen);
            }
        }

        public void OpenCopilot()
        {
            LaunchAppPackage("com.microsoft.copilot");
        }

        public void OpenGemini()
        {
            LaunchAppPackage("com.google.android.apps.bard");
        }

        private void LaunchAppPackage(string packageName)
        {
            try
            {
                Intent? intent = PackageManager?.GetLaunchIntentForPackage(packageName);
                if (intent != null)
                {
                    intent.AddFlags(ActivityFlags.NewTask);
                    StartActivity(intent);
                }
            }
            catch
            {
                // Fallback if app isn't installed
            }
        }
    }
}