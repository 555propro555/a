namespace GestureModifierApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnOpenSettingsClicked(object sender, EventArgs e)
        {
            try
            {
                var intent = new Android.Content.Intent(Android.Provider.Settings.ActionAccessibilitySettings);
                intent.AddFlags(Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Could not open settings: {ex.Message}", "OK");
            }
        }
    }
}