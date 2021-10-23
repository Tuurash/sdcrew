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

            if(Services.Settings.TimeZone!="UTC")
            {
                toolTimezone.Text = "AIRPORT";
            }
        }


        protected async override void OnAppearing()
        {
            _preflightServices = new PreflightServices();

            base.OnAppearing();
            (allPreflights as AllPreflights)?.PageAppearing();

            Services.Settings.FilterItems = "";



        }

        private void toolFilter_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new FlightFilterPopup());
        }

        public string Timezone { get; set; }

        private async void toolTimezone_Clicked(object sender, EventArgs e)
        {


            string action = await DisplayActionSheet("Select Timezone", "Cancel", null, "UTC", "AIRPORT");


            Timezone = action;
            Services.Settings.TimeZone = Timezone;

            OnPropertyChanged(nameof(Timezone));


            if (action == "" || action == null)
            {
                Services.Settings.TimeZone = "AIRPORT";
            }else if(action == "UTC")
            {
                toolTimezone.Text = "UTC";
                Services.Settings.TimeZone = "UTC";
            }
            else if (action == "AIRPORT")
            {
                toolTimezone.Text = "AIRPORT";
                Services.Settings.TimeZone = "AIRPORT";
            }

            MessagingCenter.Send<string, string>("MyApp", "TimeZoneChanged", "From AllpreflightViewModel");
        }
    }
}