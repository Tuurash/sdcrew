using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Essentials;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace sdcrew.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class supportPage : ContentPage
    {
        string CurrentCountry = String.Empty;
        string CurrentState = String.Empty;


        public supportPage()
        {
            this.Title = "Support";
            InitializeComponent();

            Device.BeginInvokeOnMainThread(() =>
            {
                Loader.IsVisible = false;
            });
        }

        #region Misc_Functions


        async void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        async void SD_Docs_Tapped(System.Object sender, System.EventArgs e)
        {
            await Browser.OpenAsync("https://sdpro.satcomdirect.com/Support/Documents", BrowserLaunchMode.SystemPreferred);
        }

        void support_Global_Tapped(System.Object sender, System.EventArgs e)
        {
            MakeCall("13217773236");
        }

        async void supportMail_1_Tapped(System.Object sender, System.EventArgs e)
        {
            await SendEmail("support@satcomdirtect.com");
        }

        async void mailGlobal_2_Tapped(System.Object sender, System.EventArgs e)
        {
            await SendEmail("milgovsupport@satcomdirtect.com");
        }


        #endregion

        #region SupportAnywhere

        async void support_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Loader.IsVisible = true;
            });

            if (Device.RuntimePlatform == Device.Android)
            {

                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (permissionStatus.ToString() == "Denied")
                {
                    await Xamarin.Essentials.Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                if (permissionStatus.ToString() != "Denied")
                {
                    var gpsServiceStatus = Xamarin.Essentials.Geolocation.GetLocationAsync();

                    if (gpsServiceStatus.Status.ToString() == "Faulted" & permissionStatus.ToString() == "Denied")
                    {
                        await DisplayAlert("Required!", "Please Enable Location Services(GPS)", "OK");
                    }
                    else
                    {
                        await PerformSupport();
                    }
                }
                else
                {
                    await DisplayAlert("Required!", "Location Permission Required.", "OK");
                }
            }
            else
            {
                try
                {
                    await PerformSupport();
                }
                catch (Exception)
                {
                    await DisplayAlert("Not Available", "Location Fetching Error.", "OK");
                }
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                Loader.IsVisible = false;
            });
        }

        async Task PerformSupport()
        {
            await CurrentLocation();
            await GetSupport(CurrentCountry);
        }

        private async Task GetSupport(string CountryName)
        {

            string CurrentLoc = String.Empty;
            if (!String.IsNullOrEmpty(CurrentState))
            {
                CurrentLoc = CurrentState;
            }
            else { CurrentLoc = CurrentCountry + " FSE"; }

            var SupportBody = CountrySupports().Where(x => x.CountryName.ToLower() == CountryName.ToLower()).FirstOrDefault();

            string getCountry = await DisplayActionSheet("Contact Support Engineer\nChoose Place", "Cancel", null, CurrentLoc);

            if (getCountry != "Cancel")
            {
                if (!String.IsNullOrEmpty(getCountry))
                {
                    await SendEmail(SupportBody.Mail);
                }
            }
        }

        #endregion

        #region locationTracker


        CancellationTokenSource cts;

        public async Task CurrentLocation()
        {
            #region geoLocation

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var locationCurrent = await Geolocation.GetLocationAsync(request, cts.Token);

            double lat = 0;
            double lon = 0;

            if (locationCurrent == null)
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    lat = location.Latitude;
                    lon = location.Longitude;
                }
            }
            else
            {
                Console.WriteLine($"Latitude: {locationCurrent.Latitude}, Longitude: {locationCurrent.Longitude}");
                lat = locationCurrent.Latitude;
                lon = locationCurrent.Longitude;
            }

            #endregion

            CurrentCountry = await getCountry(lat, lon) ?? "";
        }

        private async Task<string> getCountry(double lat, double lon)
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(lat, lon);
            var placemark = placemarks?.FirstOrDefault();

            Console.WriteLine(placemark.CountryName);

            CurrentState = placemark.AdminArea + " FSE";

            return placemark.CountryName;
        }

        private List<CountryFSE> CountrySupports()
        {
            List<CountryFSE> countryFSEs = new List<CountryFSE>();
            string text = "";

            Stream stream = this.GetType().Assembly.GetManifestResourceStream("sdcrew.Services.Data.countryFSE.json");

            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            var c = JsonConvert.DeserializeObject<List<List<Object>>>(text);

            foreach (var item in c)
            {

                var e = item.LastOrDefault().ToString().Split('(')[0];

                var country = new CountryFSE
                {
                    CountryName = item.FirstOrDefault().ToString(),
                    SupportName = item.LastOrDefault().ToString().Split('(')[0],
                    Mail = item.LastOrDefault().ToString().Split('(')[1].Trim('(', ')'),
                };

                countryFSEs.Add(country);
            }
            return countryFSEs;
        }



        #endregion

        #region Functionalities

        public async Task SendEmail(string recipient)
        {
            List<string> rcpnts = new List<string>();
            rcpnts.Add(recipient);

            List<string> CCrcpnts = new List<string>();
            CCrcpnts.Add("Support@satcomdirect.com"); //[ "Support@satcomdirect.com" ]

            try
            {
                var message = new EmailMessage
                {
                    To = rcpnts,
                    Cc = CCrcpnts,
                };
                await Email.ComposeAsync(message);
            }
            catch (Exception)
            {
                // Email is not supported on this device
            }
        }

        public void MakeCall(string PhoneNumber)
        {
            PhoneDialer.Open(PhoneNumber);
        }

        #endregion

        async void RssNotifications_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RssNotifcationPage());
        }
    }

    public class CountryFSE
    {
        public string CountryName { get; set; }
        public string SupportName { get; set; }
        public string Mail { get; set; }
    }



}