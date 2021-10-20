using Rg.Plugins.Popup.Pages;
using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlightServices : PopupPage
    {
        public string ServiceType { get; set; }

        int ScheduledAircraftTripId = 0;

        public PreflightServices preflightServices;

        public FlightServices(int scheduledAircraftTripId, string serviceType)
        {
            InitializeComponent();
            ServiceType = serviceType;
            lblServiceTilte.Text = ServiceType;

            ScheduledAircraftTripId = scheduledAircraftTripId;

            preflightServices = new PreflightServices();


            GetService();

        }

        //public ObservableCollection<Crew> CrewsObservable { get => GetService(); }

        private async void GetService()
        {
            var list= preflightServices.getServiceCrews(ScheduledAircraftTripId, ServiceType);

            listServicesCrew.ItemsSource = list;

            //var CrewObservableCollection = new ObservableCollection<Crew>(list);

        }
    }
}