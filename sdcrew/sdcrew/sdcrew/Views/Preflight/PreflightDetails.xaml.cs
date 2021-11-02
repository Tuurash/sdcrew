using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Repositories.PreflightRepos;
using sdcrew.Services.Data;
using sdcrew.ViewModels.Preflight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreflightDetails : ContentPage, INotifyPropertyChanged
    {
        public List<Tab> AllTabs { get; set; }
        dynamic flight = new PreflightVM();
        private PreflightDetailsViewModel preflightViewModel;
        PreflightServices svm;

        PreflightRepository preflightRepository = new PreflightRepository();


        public PreflightDetails(object flightObj)
        {
            InitializeComponent();
            BindingContext = this;

            AllTabs = new List<Tab>(Tabs.Get());
            collectionViewListHorizontal.ItemsSource = AllTabs;

            flight = flightObj;
            preflightViewModel = new PreflightDetailsViewModel();
            preflightRepository = new PreflightRepository();
            svm = new PreflightServices();

            lstCrewIDs.Clear();
            lstBusinessPersonIds.Clear();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                MessagingCenter.Subscribe<App>((App)Application.Current, "OnChecklistChanged", (sender) =>
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        Loader.IsVisible = true;
                        await CheckAllChecklistSStatus(); //.ContinueWith(x=> Loader.IsVisible = false)
                    });
                });
            }
            catch { }

            MainThread.BeginInvokeOnMainThread(async() => await CheckAllChecklistSStatus());
            
            await FillFlightInfo();
            var t2= FillAirport();
            var t3= FillCrews();
            var t4= FillPassengers();

            await Task.WhenAll(t2, t3, t4)
                .ContinueWith(x =>
                {
                    svm.AddServices_Preflight(flight.scheduledAircraftTripId);
                });
        }

        private async Task CheckAllChecklistSStatus()
        {
            //await FillPilotStatus();
            //await FillMaintenaceStatus();
            //await FillSchedulingStatus();
            await Task.WhenAll(FillPilotStatus(), FillMaintenaceStatus(), FillSchedulingStatus());

            MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
        }

        private async Task FillPilotStatus()
        {
            //Scheduling, Maintenance, Pilot
            var pilotStatus = await svm.CheckStatusChecklist(flight, "Pilot");

            // &#xf11a; => \uf11a  \uE80d;  "\uE80f;  \uE80e;
            if (pilotStatus == 0)
            {
                lblpilotStatus.Text = "\uE80d";
                lblpilotStatus.TextColor = Color.FromHex("FF646A");
            }
            else
            {
                if (pilotStatus == 1)
                {
                    lblpilotStatus.Text = "\uE80e"; //uE80e
                    lblpilotStatus.TextColor = Color.FromHex("33E850");
                }
                else
                {
                    lblpilotStatus.Text = "\uE80f";
                    lblpilotStatus.TextColor = Color.FromHex("FFFF6C");
                }
            }
        }

        private async Task FillMaintenaceStatus()
        {
            //Scheduling, Maintenance, Pilot

            var MaintenanceStatus = await svm.CheckStatusChecklist(flight, "Maintenance");

            // &#xf11a; => \uf11a  \uE80d;  "\uE80f;  \uE80e;
            if (MaintenanceStatus == 0)
            {
                lblMaintenanceStatus.Text = "\uE80d";
                lblMaintenanceStatus.TextColor = Color.FromHex("FF646A");
            }
            else
            {
                if (MaintenanceStatus == 1)
                {
                    lblMaintenanceStatus.Text = "\uE80e";
                    lblMaintenanceStatus.TextColor = Color.FromHex("33E850");
                }
                else
                {
                    lblMaintenanceStatus.Text = "\uE80f";
                    lblMaintenanceStatus.TextColor = Color.FromHex("FFFF6C");
                }
            }
        }

        private async Task FillSchedulingStatus()
        {

            var SchedulingStatus = await svm.CheckStatusChecklist(flight, "Scheduling");

            // &#xf11a; => \uf11a  \uE80d;  "\uE80f;  \uE80e;
            if (SchedulingStatus == 0)
            {
                lblSchedulingStatus.Text = "\uE80d";
                lblSchedulingStatus.TextColor = Color.FromHex("FF646A");
            }
            else
            {
                if (SchedulingStatus == 1)
                {
                    lblSchedulingStatus.Text = "\uE80e";
                    lblSchedulingStatus.TextColor = Color.FromHex("33E850");
                }
                else
                {
                    lblSchedulingStatus.Text = "\uE80f";
                    lblSchedulingStatus.TextColor = Color.FromHex("FFFF6C");
                }
            }
        }

        public List<int> lstCrewIDs = new List<int>();
        public List<int> lstBusinessPersonIds = new List<int>();

        #region dinamic Height

        //Bug issue: https://github.com/xamarin/Xamarin.Forms/issues/5942
        private int crewRowHeigtht;
        public int CrewRowHeigtht
        {
            get { return crewRowHeigtht; }
            set
            {
                crewRowHeigtht = value;
                RaisePropertyChanged(nameof(crewRowHeigtht));
            }
        }

        private int passengerRowHeigtht;
        public int PassengerRowHeigtht
        {
            get { return passengerRowHeigtht; }
            set
            {
                passengerRowHeigtht = value;
                RaisePropertyChanged(nameof(passengerRowHeigtht));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion

        //HeaderCollecionView
        Frame LastTab = null;

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            Frame tappedFrame = (sender as Frame);

            tappedFrame.BackgroundColor = Color.FromHex("#00AEEF");
            if (LastTab != null)
            {
                LastTab.BackgroundColor = Color.FromHex("#192E48");
                LastTab = tappedFrame;
            }
            else
            {
                LastTab = tappedFrame;
            }

            var param = ((TappedEventArgs)e).Parameter;
            var FrameName = this.FindByName<Frame>(param.ToString());
            DetailTabs.ScrollToAsync((Element)FrameName, position: ScrollToPosition.Center, animated: true);
        }



        public async Task FillFlightInfo()
        {
            PreflightVM vm = new PreflightVM();
            vm = svm.GetFlightInfo(flight.scheduledAircraftTripId);
            await Task.Delay(5);

            lblAircraftTail.Text = vm.tailNumber;
            lblFlightTripID.Text = flight.TripID;
            lblDepartureICAO.Text = flight.departureAirportIcao;
            lblArrivalICAO.Text = flight.arrivalAirportIcao;

            lblDepartHeader.Text = flight.departureAirportIcao;
            lblArrivalHeader.Text = flight.arrivalAirportIcao;

            lblDepartureDate.Text = flight.StartTime.Date.ToString("dd MMM yyyy");
            lblArrivalDate.Text = flight.EndTime.Date.ToString("dd MMM yyyy");

            lblDepartureTime.Text = flight.StartTime.ToString("H:mm") + " UTC";
            lblArrivalTime.Text = flight.EndTime.ToString("H:mm") + " UTC";
            lblETE.Text = flight.ete;

            iconFlight.TextColor = Color.FromHex(vm.color);
            lblIconHeader.TextColor = Color.FromHex(vm.color);
        }

        public async Task FillCrews()
        {

            var lst =await svm.GetCrewMembers(flight.aircraftId, flight.legNumber, flight.FID, flight.scheduledAircraftTripId);

            CrewRowHeigtht = lst.Count() * 62;
            RaisePropertyChanged(nameof(CrewRowHeigtht));

            lblCrewCount.Text = lst.Count().ToString();
            CollectionCrew.ItemsSource = lst;

            foreach (var item in lst)
            {
                lstCrewIDs.Add(item.id);
            }
        }

        public async Task FillPassengers()
        {
            var lst = await svm.GetPassengers(flight.aircraftId, flight.legNumber, flight.FID, flight.scheduledAircraftTripId);

            PassengerRowHeigtht = lst.Count() * 100;
            RaisePropertyChanged(nameof(PassengerRowHeigtht));

            lblPassengerCount.Text = lst.Count().ToString();
            lblPassengerOnboardCount.Text = svm.GetPassengersOnboard(flight.scheduledAircraftTripId, flight.legNumber).ToString();

            CollectionPassenger.ItemsSource = lst;

            foreach (var item in lst)
            {
                lstBusinessPersonIds.Add(item.businessPersonId);
            }
        }

        public async Task FillAirport()
        {
            await Task.Delay(10);

            lblAirportDepartICAO.Text = flight.departureAirportIcao;
            lblFboDepartHandlerName.Text = flight.DeparturefboHandlerName;
            lblFboDepartPhone.Text = flight.DeparturelocalPhone;
            lblFboDepartMail.Text = flight.DepartureserviceEmailAddress;

            if (lblFboDepartPhone.Text == "NA" || lblFboDepartPhone.Text == null)
            {
                lblFboDepartPhone.IsVisible = false;
                lblFboDepartCallIcon.IsVisible = false;
            }

            if (lblFboDepartMail.Text == "NA" || lblFboDepartMail.Text == null)
            {
                lblFboDepartMail.IsVisible = false;
                lblFboDepartMailIcon.IsVisible = false;
            }
            if (lblFboDepartHandlerName.Text == "NA" || lblFboDepartHandlerName.Text == null)
            {
                lblFboDepartHandlerName.IsVisible = false;
            }

            lblAirportArrival.Text = flight.arrivalAirportIcao;
            lblFboArrivalHandlerName.Text = flight.ArrvailfboHandlerName;
            lblFboArrivalPhone.Text = flight.ArrivallocalPhone;
            lblFboArrivalMail.Text = flight.ArrivalserviceEmailAddress;

            if (lblFboArrivalPhone.Text == "NA" || lblFboArrivalPhone.Text == null)
            {
                lblFboArrivalPhone.IsVisible = false;
                lblFboArrivalCallIcon.IsVisible = false;
            }


            if (lblFboArrivalMail.Text == "NA" || lblFboArrivalMail.Text == null)
            {
                lblFboArrivalMail.IsVisible = false;
                lblFboArrivalMailIcon.IsVisible = false;
            }

            if (lblFboArrivalHandlerName.Text == "NA" || lblFboArrivalHandlerName.Text == null)
            {
                lblFboArrivalHandlerName.IsVisible = false;
            }
        }



        private void btnCheckList_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
            //Thread.Sleep(2500);
            Task.Run(() => PopupNavigation.Instance.PushAsync(new FlightCheckList(flight.scheduledAircraftTripId, flight.legNumber, flight.scheduledFlightId))).ConfigureAwait(false);
        }

        private void btnServices_Tapped(object sender, EventArgs e)
        {
            var ServiceType = ((TappedEventArgs)e).Parameter;
            Task.Run(() => PopupNavigation.Instance.PushAsync(new FlightServices(flight.scheduledAircraftTripId, ServiceType.ToString())));
        }

        private void btnBack_Tapped(object sender, EventArgs e)
        {

            Application.Current.MainPage.Navigation.PopToRootAsync();
            //await Navigation.PushAsync(new );
        }

        private async void FABItenery_Clicked(object sender, EventArgs e)
        {

            string getType = await DisplayActionSheet("", "Cancel", null, "View Crew Itinerary", "View Pax Itinerary");

            int type = 1;

            MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            if (getType == "View Crew Itinerary")
            {
                type = 1;
            }
            else if (getType == "View Pax Itinerary") { type = 2; } else { return; }

            ArrayList CrewIDarrayList = new ArrayList(lstCrewIDs);

            ArrayList BusinessPersonIdarrayList = new ArrayList(lstBusinessPersonIds);

            int legNumber = flight.legNumber;

            await Task.Run(async () => await preflightRepository.Fetch_Itineray(type, legNumber, CrewIDarrayList, BusinessPersonIdarrayList, flight.scheduledAircraftTripId))
                .ContinueWith(x =>
                {
                    MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
                });

        }

        #region Call&Mails

        public async Task MakeCall(string PhoneNumber)
        {
            string number = PhoneNumber;
            await Task.Delay(100);
            MainThread.BeginInvokeOnMainThread(() => PhoneDialer.Open(number));
        }

        public async Task OpenInMail(string MailAddress)
        {
            var address = MailAddress;

            await Task.Delay(100);
            MainThread.BeginInvokeOnMainThread(() => Launcher.TryOpenAsync(new Uri($"mailto:{address}")));
        }


        private void crewMakeCall_Tapped(object sender, EventArgs e)
        {
            var PhoneNumber = ((TappedEventArgs)e).Parameter;
            MainThread.BeginInvokeOnMainThread(async () => await MakeCall(PhoneNumber.ToString()));

        }

        private void AirportSendMail_Tapped(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () => await OpenInMail(flight.ArrivalserviceEmailAddress));

        }

        private void btnAirportDepurtureCall_Tapped(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () => await MakeCall(flight.DeparturelocalPhone));
        }

        private void btnAirportDepurtureMail_Tapped(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () => await OpenInMail(flight.DepartureserviceEmailAddress));
        }

        private void btnAirportArrivalCall_Tapped(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () => await MakeCall(flight.ArrivallocalPhone));
        }


        #endregion

    }

    public class Tab
    {
        public string TabHeader { get; set; }
        public int TabIndex { get; set; }

        public string TabColor { get; set; }
    }

    public class Tabs
    {
        public static IEnumerable<Tab> Get()
        {
            string color = "#192E48";

            return new List<Tab>
            {
                new Tab() {TabHeader="FLIGHT", TabIndex=1,TabColor=color},
                new Tab() {TabHeader="AIRPORTS", TabIndex=2,TabColor=color},
                new Tab() {TabHeader="CREWS", TabIndex=3,TabColor=color},
                new Tab() {TabHeader="PASSENGERS", TabIndex=4,TabColor=color},
                new Tab() {TabHeader="CHECKLISTS", TabIndex=5,TabColor=color},
                new Tab() {TabHeader="SERVICES", TabIndex=6,TabColor=color},
            };
        }

    }
}