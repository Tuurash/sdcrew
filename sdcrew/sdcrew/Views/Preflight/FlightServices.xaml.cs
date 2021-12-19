using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class FlightServices : PopupPage, INotifyPropertyChanged
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

        private string vendorName = "";
        public string VendorName { get { return vendorName; } set { vendorName = value; OnPropertyChanged(); } }

        private string phone = "";
        public string Phone { get { return phone; } set { phone = value; OnPropertyChanged(); } }

        private string url = "";
        public string Url { get { return url; } set { url = value; OnPropertyChanged(); } }

        private string generalNotes = "";
        public string GeneralNotes { get { return generalNotes; } set { generalNotes = value; OnPropertyChanged(); } }

        //ServiceUrlVisibility      PhoneIconVisibility
        private string serviceVisibility = "false";
        public string ServiceVisibility { get { return generalNotes; } set { generalNotes = value; OnPropertyChanged(); } }



        private async Task GetService()
        {

            var list = await preflightServices.getServiceCrews(ScheduledAircraftTripId, ServiceType);

            if(list.Count() > 0)
            {
                ServiceVisibility = "true";

                VendorName = list.Select(x => x.vendorName).FirstOrDefault();
                Phone = list.Select(x => x.phone).FirstOrDefault();
                Url = list.Select(x => x.url).FirstOrDefault();
                GeneralNotes = list.Select(x => x.generalNotes).FirstOrDefault();

                listServicesCrew.ItemsSource = list;
            }
            else
            {
                ServiceVisibility = "false";
            }
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