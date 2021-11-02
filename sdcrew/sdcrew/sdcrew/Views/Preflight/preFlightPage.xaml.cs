using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using sdcrew.ViewModels.Preflight;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class preFlightPage : ContentPage
    {
        PreflightServices _preflightServices;

        private string time = Services.Settings.TimeZone;

        public preFlightPage()
        {
            InitializeComponent();

            if(Services.Settings.TimeZone == null)
            {
                Services.Settings.TimeZone = "UTC";
            }
            else if(Services.Settings.TimeZone == "UTC")
            {
                toolTimezone.Text = "UTC";

            }else if(Services.Settings.TimeZone == "AIRPORT")
            {
                toolTimezone.Text = "AIRPORT";
            }
        }


        protected async override void OnAppearing()
        {
            _preflightServices = new PreflightServices();

            base.OnAppearing();
            (allPreflights as AllPreflights)?.PageAppearing();
        }

        private void toolFilter_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new FlightFilterPopup());
        }

        public string Timezone { get; set; }

        private async void toolTimezone_Clicked(object sender, EventArgs e)
        {


            string action = await DisplayActionSheet("Choose Time Zone", "Cancel", null, "UTC", "AIRPORT");

            if (action == "" || action == null)
            {
                toolTimezone.Text = "AIRPORT";
            }
            else
            {
                toolTimezone.Text = action;
            }

            await TimezoneChanged(action).ConfigureAwait(false);
        }

        private async Task TimezoneChanged(string action)
        {
            Services.Settings.TimeZone = action;
            await Task.Delay(20);

            if (action == "" || action == null)
            {
                Services.Settings.TimeZone = "AIRPORT";
            }
            else if (action == "UTC")
            {
                Services.Settings.TimeZone = "UTC";
            }
            else if (action == "AIRPORT")
            {
                Services.Settings.TimeZone = "AIRPORT";
            }

            MessagingCenter.Send<string, string>("MyApp", "TimeZoneChanged", "From AllpreflightViewModel");
        }
    }
}