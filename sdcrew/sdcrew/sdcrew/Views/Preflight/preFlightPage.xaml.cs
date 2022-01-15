using Microsoft.AppCenter.Crashes;

using Rg.Plugins.Popup.Services;

using sdcrew.Services;
using sdcrew.Services.Data;
using sdcrew.ViewModels;

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class preFlightPage : ContentPage
    {
        private string time = Services.Settings.TimeZone;

        public preFlightPage()
        {
            InitializeComponent();

            //Crashes.GenerateTestCrash();

            if (Services.Settings.TimeZone == null)
            {
                Services.Settings.TimeZone = "UTC";
                Services.Settings.setAirportLocalTimeBool = "false";
            }
            else if (Services.Settings.TimeZone == "UTC")
            {
                toolTimezone.Text = "UTC";
                Services.Settings.setAirportLocalTimeBool = "false";


            }
            else if (Services.Settings.TimeZone == "AIRPORT")
            {
                toolTimezone.Text = "AIRPORT";
                Services.Settings.setAirportLocalTimeBool = "true";
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<string>(this, "SelectedTabFromFlyout", (ob) =>
            {
                if (ob != "FlightTab")
                {
                    PreflightTabs.SelectedIndex = 2;
                }
                else
                {
                    PreflightTabs.SelectedIndex = 1;
                }
            });
            if (!string.IsNullOrEmpty(Services.Settings.UpdateNotification) & Services.Settings.UpdateNotification != "False")
            {
                await CheckVersion();
            }
        }

        private async Task CheckVersion()
        {

            VersionTracker versionTracker = new VersionTracker();

            var Islatest = await versionTracker.CheckForUpdate();
            if (Islatest != 0)
            {
                var update = await DisplayAlert("New Version", "There is a new version of this app available. Would you like to update now?", "Yes", "No");

                if (update)
                {
                    await versionTracker.OpenAppInStore();
                }
            }
        }

        private async void toolFilter_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            await PopupNavigation.Instance.PushAsync(new FlightFilterPopup());
        }

        public string Timezone { get; set; }

        private async void toolTimezone_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            string action = await DisplayActionSheet("Choose Time Zone", "Cancel", null, "UTC", "AIRPORT");

            if (action == "" || action == null)
            {
                toolTimezone.Text = "AIRPORT";
            }
            else if (action == "UTC")
            {
                toolTimezone.Text = action;
            }
            else if (action == "AIRPORT")
            {
                toolTimezone.Text = action;
            }
            else { }

            await TimezoneChanged(action).ConfigureAwait(false);
        }

        private async Task TimezoneChanged(string action)
        {
            Services.Settings.TimeZone = action;
            await Task.Delay(1);

            if (action == "" || action == null)
            {
                //Services.Settings.TimeZone = "AIRPORT";
                //Services.Settings.setAirportLocalTimeBool = "true";
            }
            else if (action == "UTC")
            {
                Services.Settings.TimeZone = "UTC";
                Services.Settings.setAirportLocalTimeBool = "false";

                MessagingCenter.Send<string, string>("MyApp", "TimeZoneChanged", "From AllpreflightViewModel");
            }
            else if (action == "AIRPORT")
            {
                Services.Settings.TimeZone = "AIRPORT";
                Services.Settings.setAirportLocalTimeBool = "true";

                MessagingCenter.Send<string, string>("MyApp", "TimeZoneChanged", "From AllpreflightViewModel");
            }

        }
    }
}