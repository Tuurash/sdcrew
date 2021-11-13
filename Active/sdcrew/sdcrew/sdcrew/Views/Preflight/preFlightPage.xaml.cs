using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using sdcrew.ViewModels.Preflight;
using sdcrew.Views.Login;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class preFlightPage : ContentPage
    {
        private string time = Services.Settings.TimeZone;

        public preFlightPage()
        {
            InitializeComponent();

            if(Services.Settings.TimeZone == null)
            {
                Services.Settings.TimeZone = "UTC";
                Services.Settings.setAirportLocalTimeBool = "false";
            }
            else if(Services.Settings.TimeZone == "UTC")
            {
                toolTimezone.Text = "UTC";
                Services.Settings.setAirportLocalTimeBool = "false";


            }
            else if(Services.Settings.TimeZone == "AIRPORT")
            {
                toolTimezone.Text = "AIRPORT";
                Services.Settings.setAirportLocalTimeBool = "true";
            }
        }


        protected async override void OnAppearing()
        {
            credentialsViewModel credentials = new credentialsViewModel();
            UserServices userServices = new UserServices();

            base.OnAppearing();
            (allPreflights as AllPreflights)?.PageAppearing();

            await Task.Delay(1);
            //will need it one day

            //await Task.Run(async () =>
            //{
            //    int tokenFlag = await credentials.PerformRefreshToken();
            //    if (tokenFlag == 1)
            //    {
            //        await userServices.AddUser();
            //    }
            //    else
            //    {
            //        App.Current.MainPage = new loginPage();
            //    }
            //}).ConfigureAwait(false);
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
            else if(action != "Cancel")
            {
                toolTimezone.Text = action;
            }

            await TimezoneChanged(action).ConfigureAwait(false);
        }

        private async Task TimezoneChanged(string action)
        {
            Services.Settings.TimeZone = action;
            await Task.Delay(1);

            if (action == "" || action == null)
            {
                Services.Settings.TimeZone = "AIRPORT";
                Services.Settings.setAirportLocalTimeBool = "true";
            }
            else if (action == "UTC")
            {
                Services.Settings.TimeZone = "UTC";
                Services.Settings.setAirportLocalTimeBool = "false";
            }
            else if (action == "AIRPORT")
            {
                Services.Settings.TimeZone = "AIRPORT";
                Services.Settings.setAirportLocalTimeBool = "true";
            }

            MessagingCenter.Send<string, string>("MyApp", "TimeZoneChanged", "From AllpreflightViewModel");
        }
    }
}