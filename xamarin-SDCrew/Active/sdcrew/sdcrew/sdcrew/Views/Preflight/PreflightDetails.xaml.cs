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

        protected override void OnAppearing()
        {
            base.OnAppearing();


            FillAirport();
            FillFlightInfo();
            FillCrews();
            FillPassengers();

            Task.Run(() =>
            {
                svm.AddServices_Preflight(flight.scheduledAircraftTripId);

            }).ConfigureAwait(false);
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
        Frame LastTab=null;

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

        public void FillFlightInfo()
        {
            PreflightVM vm = new PreflightVM();
            vm = preflightViewModel.GetFlightInfo(flight.aircraftId);

            lblAircraftTail.Text = vm.tailNumber;
            lblFlightTripID.Text = vm.TripID;
            lblDepartureICAO.Text = vm.departureAirportIcao;
            lblArrivalICAO.Text = vm.arrivalAirportIcao;

            lblDepartHeader.Text = vm.departureAirportIcao;
            lblArrivalHeader.Text = vm.arrivalAirportIcao;

            lblDepartureDate.Text = vm.startDateTimeUtc.Date.ToString("MMMM yyyy");
            lblArrivalDate.Text = vm.endDateTimeUtc.Date.ToString("MMMM yyyy");

            lblDepartureTime.Text = vm.startDateTimeUtc.ToString("H:mm") + " UTC";
            lblArrivalTime.Text = vm.endDateTimeUtc.ToString("H:mm") + " UTC";
            lblETE.Text = "ETE-" + vm.ete;

            iconFlight.TextColor = Color.FromHex(vm.color);
            lblIconHeader.TextColor = Color.FromHex(vm.color);
        }

        public void FillCrews()
        {

            var lst = svm.GetCrewMembers(flight.aircraftId, flight.legNumber, flight.FID, flight.scheduledAircraftTripId);

            CrewRowHeigtht = lst.Count() * 50;
            RaisePropertyChanged(nameof(CrewRowHeigtht));

            lblCrewCount.Text = lst.Count().ToString();
            CollectionCrew.ItemsSource = lst;

            foreach (var item in lst)
            {
                lstCrewIDs.Add(item.id);
            }
        }

        public void FillPassengers()
        {
            var lst = svm.GetPassengers(flight.aircraftId, flight.legNumber, flight.FID, flight.scheduledAircraftTripId);

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

        public void FillAirport()
        {


            lblAirportDepartICAO.Text = flight.departureAirportIcao;
            lblFboDepartHandlerName.Text = flight.DeparturefboHandlerName;
            lblFboDepartPhone.Text = flight.DeparturelocalPhone;
            lblFboDepartMail.Text = flight.DepartureserviceEmailAddress;

            if (lblFboDepartPhone.Text == "" || lblFboDepartPhone.Text == null)
            {
                lblFboDepartCallIcon.IsVisible = false;
            }

            if (lblFboDepartMail.Text == "" || lblFboDepartMail.Text == null)
            {
                lblFboDepartMailIcon.IsVisible = false;
            }

            lblAirportArrival.Text = flight.departureAirportIcao;
            lblFboArrivalHandlerName.Text = flight.ArrvailfboHandlerName;
            lblFboArrivalPhone.Text = flight.ArrivallocalPhone;
            lblFboArrivalMail.Text = flight.ArrivalserviceEmailAddress;

            if (lblFboArrivalPhone.Text == "" || lblFboArrivalPhone.Text == null)
            {
                lblFboArrivalCallIcon.IsVisible = false;
            }


            if (lblFboArrivalMail.Text == "" || lblFboArrivalMail.Text == null)
            {
                lblFboArrivalMailIcon.IsVisible = false;
            }

        }

        private void btnCheckList_Tapped(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            Thread.Sleep(2000);
            Task.Run(() => PopupNavigation.Instance.PushAsync(new FlightCheckList(flight.scheduledAircraftTripId, flight.legNumber, flight.scheduledFlightId)));

            Loader.IsVisible = false;
        }

        private void btnServices_Tapped(object sender, EventArgs e)
        {
            var ServiceType = ((TappedEventArgs)e).Parameter;
            Task.Run(() => PopupNavigation.Instance.PushAsync(new FlightServices(flight.scheduledAircraftTripId, ServiceType.ToString())));
        }

        private void btnBack_Tapped(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void FABItenery_Clicked(object sender, EventArgs e)
        {

            string getType = await DisplayActionSheet("", "Cancel", null, "Crew", "PAX");

            int type = 1;

            if (getType == "Crew")
            {
                type = 1;
            }
            else if (getType == "PAX") { type = 2; } else { return; }

            ArrayList CrewIDarrayList = new ArrayList(lstCrewIDs);

            ArrayList BusinessPersonIdarrayList = new ArrayList(lstBusinessPersonIds);

            int legNumber = flight.legNumber;

            try
            {
                await Task.Run(async () => await preflightRepository.Fetch_Itineray(type, legNumber, CrewIDarrayList, BusinessPersonIdarrayList, flight.scheduledAircraftTripId));
            }
            catch (Exception exc) { Debug.WriteLine(exc); }

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