using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlightServices : PopupPage,INotifyPropertyChanged
    {
        public string ServiceType { get; set; }

        int ScheduledAircraftTripId = 0;

        public PreflightServices preflightServices;

        public FlightServices(int scheduledAircraftTripId, string serviceType)
        {
            InitializeComponent();
            BindingContext = this;

            ServiceType = serviceType;
            lblServiceTilte.Text = ServiceType;

            ScheduledAircraftTripId = scheduledAircraftTripId;
            preflightServices = new PreflightServices();

            MainThread.BeginInvokeOnMainThread(async () => await GetService());

        }

        private async void btnColosePopup_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private string vendorName="SomeVendor";
        public string VendorName{get{ return vendorName;}set{ vendorName = value; OnPropertyChanged();}}

        private string phone = "phone";
        public string Phone { get { return phone; } set { phone = value; OnPropertyChanged(); } }

        private string url = "url";
        public string Url { get { return url; } set { url = value; OnPropertyChanged(); } }

        private string generalNotes = "general Notesss";
        public string GeneralNotes { get { return generalNotes; } set { generalNotes = value; OnPropertyChanged(); } }

        private async Task GetService()
        {

            var list =await preflightServices.getServiceCrews(ScheduledAircraftTripId, ServiceType);
            VendorName = list.Select(x => x.vendorName).FirstOrDefault();
            Phone = list.Select(x => x.phone).FirstOrDefault();
            Url = list.Select(x => x.url).FirstOrDefault();
            GeneralNotes = list.Select(x => x.generalNotes).FirstOrDefault();


            listServicesCrew.ItemsSource = list;
        }

        void ServiceCrewVendorPhone_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => PhoneDialer.Open(Phone));
        }

        void ServiceCrewVendorUrl_Tapped(System.Object sender, System.EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Launcher.OpenAsync(new Uri("https://" + Url)));
        }
    }

}