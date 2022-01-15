using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Services;

using sdcrew.Models;

using sdcrew.Services.Data;
using sdcrew.Views.Postflight.Modals.DropdownModals;
using sdcrew.Views.Postflight.SubViews;

using Xamarin.CommunityToolkit;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class PostflightDetails : ContentPage, INotifyPropertyChanged
    {
        public List<Tab> AllTabs { get; set; }
        dynamic flight = new PostFlightVM();

        PostflightServices svm;

        bool IsRunning = false;
        string SubNavItemString = "";

        public PostflightDetails(dynamic flightObj)
        {
            InitializeComponent();
            BindingContext = this;

            #region SubNavVisibility

            FillSubNav();

            if (!String.IsNullOrEmpty(Services.Settings.PostflightSubviewVisibility))
            {
                postflighSubNav.IsVisible = bool.Parse(Services.Settings.PostflightSubviewVisibility);
            }
            else
            {
                postflighSubNav.IsVisible = true;
            }

            //PostflightSubNavItemsUpdate
            MessagingCenter.Subscribe<string>(this, "PostflightSubNavItemsUpdated", (ob) =>
            {
                FillSubNav();
            });

            MessagingCenter.Subscribe<string>(this, "PostflightSubviewUpdated", (ob) =>
            {
                postflighSubNav.IsVisible = bool.Parse(Services.Settings.PostflightSubviewVisibility);
            });

            #region Synchronization

            if (String.IsNullOrWhiteSpace(Services.Settings.HasLocalUpdates))
            {
                logoSynchStatus.TextColor = Color.FromHex("#99E3FF");
                lblSyncStatus.Text = "Synched";
            }
            else
            {
                if (Services.Settings.HasLocalUpdates == "True")
                {
                    logoSynchStatus.TextColor = Color.FromHex("#FFA621");
                    lblSyncStatus.Text = "Changes";
                }
            }


            MessagingCenter.Subscribe<App>((App)Application.Current, "OnLocalModification", (sender) =>
            {
                logoSynchStatus.TextColor = Color.FromHex("#FFA621");
                lblSyncStatus.Text = "Changes";
            });

            #endregion


            #endregion

            flight = flightObj;

            svm = new PostflightServices();

            FillNavbarInfo();
            //Task.Run(() =>
            //{
            //    FillNavbarInfo();
            //});

            FillOooiDetails();
            FillFuelDetails();
            FillCrews();
            FillPassengers();


            MessagingCenter.Subscribe<AllAirports>(this, "airportSelectedData", (ob) =>
            {
                AllAirports receivedData = ob;
            });

            MessagingCenter.Subscribe<AllAirports>(this, "OooiDeparture", (ob) =>
            {
                AllAirports receivedData = ob;
                dropdownDepartOooi.Text = receivedData.airportCode;
            });

            MessagingCenter.Subscribe<AllAirports>(this, "OooiArrive", (ob) =>
            {
                AllAirports receivedData = ob;
                dropdownArriveOooi.Text = receivedData.airportCode;
            });



            MessagingCenter.Subscribe<string>(this, "CrewRemoved", async (ob) =>
             {
                 int CGR = int.Parse(ob);
                 //Console.WriteLine("Rowwwwwww" + CrewGridRow);
                 await RemoveCrewGridRow(CGR);
                 expanderCrew.ForceUpdateSize();
             });

            MessagingCenter.Subscribe<App>((App)Application.Current, "CrewUpdated", (sender) =>
            {
                expanderCrew.ForceUpdateSize();
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "SquawkUpdated", (sender) =>
            {
                ExpanderSquawk.ForceUpdateSize();
            });
        }



        #region Navbar nd Tabs

        private void FillNavbarInfo()
        {
            lblTail.Text = flight.TailNumber;
            lblDepartHeader.Text = flight.departureIcao;
            lblDepartDate.Text = flight.StartDate;

            lblIconHeader.TextColor = Color.FromHex(flight.aircraftColor);
            lblTripId.Text = "Trip ID: " + flight.Customized_TripId.Split('-')[0];
            lblArrivalHeader.Text = flight.airportIcao;
            lblArrivalDate.Text = flight.EndDate;
        }

        private void FillSubNav()
        {
            List<Tab> AllTabs = new List<Tab>();

            SubNavItemString = Services.Settings.PostflightSubNavItems + ",OOOI,FUEL,CREW,";
            string[] navItems = SubNavItemString.Split(',');
            var fliteredTabs = Tabs.Get().Where(x => navItems.Contains(x.TabHeader)).OrderBy(x => x.TabIndex).ToList();

            AllTabs = fliteredTabs;

            collectionViewListHorizontal.ItemsSource = AllTabs;
        }

        //HeaderCollecionView
        Frame LastTab = null;

        void HeaderSubNav_Tapped(System.Object sender, System.EventArgs e)
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

        void btnBack_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PopAsync(true);
            });
        }

        #endregion

        #region Oooi

        private void FillOooiDetails()
        {
            //HeaderFields
            lblDepartOooi.Text = dropdownDepartOooi.Text = flight.departureIcao;
            lblArriveOooi.Text = dropdownArriveOooi.Text = flight.airportIcao;

            Oooi oooiDetails = svm.GetOooiDetails(flight.postedFlightId);

            if (oooiDetails.HasLocalModification == true)
            {
                txtOooiIn.TextColor = txtOooiOff.TextColor = txtOooiOn.TextColor = txtOooiOut.TextColor = lblOooiIn.TextColor = lblOooiOff.TextColor = lblOooiOn.TextColor = lblOooiOut.TextColor = Color.White;
            }

            lblOooiOut.Text = txtOooiOut.Text = oooiDetails.automatedOutTime.ToString("HH:mm") ?? "00:00";
            lblOooiIn.Text = txtOooiIn.Text = oooiDetails.automatedInTime.ToString("HH:mm") ?? "00:00";

            var genBlockTime = oooiDetails.automatedInTime - oooiDetails.automatedOutTime;
            lblOooiBlockTime.Text = genBlockTime.ToString(@"hh\:mm") + "/" + ((genBlockTime.TotalMinutes / 60)).ToString("0.0");

            lblOooiOff.Text = txtOooiOff.Text = oooiDetails.automatedOffTime.ToString("HH:mm") ?? "00:00";
            lblOooiOn.Text = txtOooiOn.Text = oooiDetails.automatedOnTime.ToString("HH:mm") ?? "00:00";

            var genFlightTime = oooiDetails.automatedOnTime - oooiDetails.automatedOffTime;
            lblOooiFlightTime.Text = genFlightTime.ToString(@"hh\:mm") + "/" + ((genFlightTime.TotalMinutes / 60)).ToString("0.0");

            var genTaxiTime = oooiDetails.automatedOffTime - oooiDetails.automatedOutTime;
            lblTaxiTime.Text = genTaxiTime.ToString(@"hh\:mm") + "/" + ((genTaxiTime.TotalMinutes / 60)).ToString("0.0");

            var genOnInTaxiTime = oooiDetails.automatedInTime - oooiDetails.automatedOnTime;
            lblOooiOnInTaxi.Text = genOnInTaxiTime.ToString(@"hh\:mm") + "/" + ((genOnInTaxiTime.TotalMinutes / 60)).ToString("0.0");

            if (flight.StartDate != "N/A" & flight.EndDate != "N/A")
            {
                pickerDepartDateOooi.Date = DateTime.Parse(flight.StartDate);
                pickerArriveDateOooi.Date = DateTime.Parse(flight.EndDate);
            }

            restoreOooiData1.IsVisible = restoreOooiData2.IsVisible = restoreOooiData3.IsVisible = restoreOooiData4.IsVisible = false;

        }

        int OooiChangeCount = 0;

        async void OooiTextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //var txtbox = sender as Entry;
            await Task.Delay(10);
            OooiChangeCount++;
            if (OooiChangeCount > 4)
            {
                txtOooiIn.TextColor = txtOooiOff.TextColor = txtOooiOn.TextColor = txtOooiOut.TextColor = lblOooiIn.TextColor = lblOooiOff.TextColor = lblOooiOn.TextColor = lblOooiOut.TextColor = Color.White;
            }

            if (DateTime.TryParse(txtOooiOut.Text, out DateTime TempOut) == true && DateTime.TryParse(txtOooiIn.Text, out DateTime TempIn) == true
                && DateTime.TryParse(txtOooiOff.Text, out DateTime TempOff) == true && DateTime.TryParse(txtOooiOn.Text, out DateTime TempOn) == true)
            {
                lblOooiOut.Text = TempOut.ToString("HH:mm");
                lblOooiIn.Text = TempIn.ToString("HH:mm");

                var genBlockTime = (TempIn - TempOut).Duration();
                lblOooiBlockTime.Text = genBlockTime.ToString(@"hh\:mm") + "/" + ((genBlockTime.TotalMinutes / 60)).ToString("0.0");

                lblOooiOff.Text = TempOff.ToString("HH:mm");
                lblOooiOn.Text = TempOn.ToString("HH:mm");

                var genFlightTime = (TempOn - TempOff).Duration();
                if (TempOff > TempOn)
                {
                    genFlightTime += TimeSpan.FromDays(1);
                }

                lblOooiFlightTime.Text = genFlightTime.ToString(@"hh\:mm") + "/" + ((genFlightTime.TotalMinutes / 60)).ToString("0.0");

                var genTaxiTime = (TempOff - TempOut).Duration();
                if (TempOut > TempOff)
                {
                    genTaxiTime += TimeSpan.FromDays(1);
                }
                lblTaxiTime.Text = genTaxiTime.ToString(@"hh\:mm") + "/" + ((genTaxiTime.TotalMinutes / 60)).ToString("0.0");

                var genOnInTaxiTime = (TempIn - TempOn).Duration();
                if (TempIn < TempOn)
                {
                    genOnInTaxiTime += TimeSpan.FromDays(1);
                }

                lblOooiOnInTaxi.Text = genOnInTaxiTime.ToString(@"hh\:mm") + "/" + ((genOnInTaxiTime.TotalMinutes / 60)).ToString("0.0");
            }

            restoreOooiData1.IsVisible = restoreOooiData2.IsVisible = restoreOooiData3.IsVisible = restoreOooiData4.IsVisible = true;
        }

        void dropdwnOooiDepart_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new AirportICAODropdown("OooiDeparture")));
        }

        void drpDwnOooiArrive_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new AirportICAODropdown("OooiArrive")));
        }

        void restoreOooiData_Tapped(System.Object sender, System.EventArgs e)
        {

            FillOooiDetails();
            txtOooiIn.TextColor = txtOooiOff.TextColor = txtOooiOn.TextColor = txtOooiOut.TextColor = lblOooiIn.TextColor = lblOooiOff.TextColor = lblOooiOn.TextColor = lblOooiOut.TextColor = Color.FromHex("#52E580");
        }


        #endregion

        #region Fuel
        //Check for double if null or 0
        static bool IsNullOrDefault<T>(T value)
        {
            return object.Equals(value, default(T));
        }

        int FuelChangeCount = 0;
        bool restoreFlag = true;
        private async void FillFuelDetails()
        {
            double tempFuelOut = 0;
            double? tempFuelIn = 0;

            if (restoreFlag == false)
            {
                if (String.IsNullOrEmpty(txtFuelOut.Text)) { tempFuelOut = 0; lblFuelOut.Text = "N/A"; } else { lblFuelOut.Text = txtFuelOut.Text; tempFuelOut = int.Parse(txtFuelOut.Text); }
                if (String.IsNullOrEmpty(txtFuelIn.Text)) { tempFuelIn = 0; lblFuelIn.Text = "N/A"; } else { tempFuelIn = int.Parse(txtFuelIn.Text); lblFuelIn.Text = txtFuelIn.Text; }
                if (String.IsNullOrEmpty(txtFuelOff.Text)) { lblFuelOff.Text = "N/A"; } else { lblFuelOff.Text = txtFuelOff.Text; }
                if (String.IsNullOrEmpty(txtFuelOn.Text)) { lblFuelOn.Text = "N/A"; } else { lblFuelOn.Text = txtFuelOn.Text; }
            }
            else
            {
                txtFuelBurn.TextColor = txtFuelIn.TextColor = txtFuelOff.TextColor = txtFuelOn.TextColor = txtFuelOut.TextColor = lblFuelIn.TextColor = lblFuelOff.TextColor = lblFuelOn.TextColor = lblFuelOut.TextColor = Color.FromHex("#52E580");
                PostedFlightFuel FuelDetails = await svm.GetFuelDetails(flight.postedFlightId);
                if (FuelDetails != null)
                {
                    if (IsNullOrDefault(FuelDetails.fuelOut)) { tempFuelOut = 0; txtFuelOut.Text = ""; lblFuelOut.Text = "N/A"; }
                    else { lblFuelOut.Text = txtFuelOut.Text = FuelDetails.fuelOut.ToString(); tempFuelOut = FuelDetails.fuelOut; }
                    if (IsNullOrDefault(FuelDetails.fuelIn)) { tempFuelIn = 0; txtFuelIn.Text = ""; lblFuelIn.Text = "N/A"; } else { lblFuelIn.Text = txtFuelIn.Text = FuelDetails.fuelIn.ToString(); tempFuelIn = FuelDetails.fuelIn; }
                    if (IsNullOrDefault(FuelDetails.fuelOff)) { txtFuelOff.Text = ""; lblFuelOff.Text = "N/A"; } else { lblFuelOff.Text = txtFuelOff.Text = FuelDetails.fuelOff.ToString(); }
                    if (IsNullOrDefault(FuelDetails.fuelOn)) { txtFuelOn.Text = ""; lblFuelOn.Text = "N/A"; } else { lblFuelOn.Text = txtFuelOn.Text = FuelDetails.fuelOff.ToString(); }
                }
            }

            lblFuelBurn.Text = txtFuelBurn.Text = (tempFuelOut - tempFuelIn).ToString();

            if (SegmentfuelType.SelectedSegment < 3)
            {
                string type = String.Empty;

                if (SegmentfuelType.SelectedSegment == 0)
                    type = " lbs";
                else if (SegmentfuelType.SelectedSegment == 1)
                    type = " gal";
                else
                    type = " ltr";

                lblFuelInType.Text = lblFuelOffType.Text = lblFuelOnType.Text = lblFuelOutType.Text = type;
                lblFuelBurn.Text = (tempFuelOut - tempFuelIn) + type;
            }
            else
            {
                lblFuelInType.Text = lblFuelOffType.Text = lblFuelOnType.Text = lblFuelOutType.Text = "";
            }
        }

        void SegmentfuelType_OnSegmentSelected(System.Object sender, Plugin.Segmented.Event.SegmentSelectEventArgs e)
        {
            FillFuelDetails();
        }


        async void txtFuel_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //var txtbox = sender as Entry;
            double tempOut = 0;
            double tempIn = 0;

            await Task.Delay(10);
            FuelChangeCount++;
            if (FuelChangeCount > 5)
            {
                restoreFlag = false;

                restoreOooiData5.IsVisible = restoreOooiData6.IsVisible = restoreOooiData7.IsVisible = restoreOooiData8.IsVisible = restoreOooiData9.IsVisible = true;
                txtFuelBurn.TextColor = txtFuelIn.TextColor = txtFuelOff.TextColor = txtFuelOn.TextColor = txtFuelOut.TextColor = lblFuelIn.TextColor = lblFuelOff.TextColor = lblFuelOn.TextColor = lblFuelOut.TextColor = Color.White;

                if (!String.IsNullOrEmpty(txtFuelOut.Text)) { tempOut = int.Parse(txtFuelOut.Text); }
                if (!String.IsNullOrEmpty(txtFuelIn.Text)) { tempIn = int.Parse(txtFuelIn.Text); }

                if (tempOut > tempIn)
                {
                    FillFuelDetails();
                }
                else
                {
                    txtFuelIn.TextColor = txtFuelOut.TextColor = Color.Red;
                }
            }
        }

        async void txtFuelBurn_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            double tempOut = 0;
            double tempBurn = 0;

            await Task.Delay(10);
            FuelChangeCount++;
            if (FuelChangeCount > 5)
            {
                restoreFlag = false;

                restoreOooiData5.IsVisible = restoreOooiData6.IsVisible = restoreOooiData7.IsVisible = restoreOooiData8.IsVisible = restoreOooiData9.IsVisible = true;
                txtFuelBurn.TextColor = txtFuelIn.TextColor = txtFuelOff.TextColor = txtFuelOn.TextColor = txtFuelOut.TextColor = lblFuelIn.TextColor = lblFuelOff.TextColor = lblFuelOn.TextColor = lblFuelOut.TextColor = Color.White;

                if (!String.IsNullOrEmpty(txtFuelOut.Text)) { tempOut = int.Parse(txtFuelOut.Text); }
                if (!String.IsNullOrEmpty(txtFuelBurn.Text)) { tempBurn = int.Parse(txtFuelBurn.Text); }

                if (tempOut < tempBurn)
                {
                    txtFuelBurn.TextColor = txtFuelOut.TextColor = Color.Red;
                }
                else
                {
                    txtFuelIn.Text = (tempOut - tempBurn).ToString();
                    FillFuelDetails();
                }
            }
        }

        void restoreFuel_Tapped(System.Object sender, System.EventArgs e)
        {
            restoreFlag = true;
            FuelChangeCount = 4;
            restoreOooiData5.IsVisible = restoreOooiData6.IsVisible = restoreOooiData7.IsVisible = restoreOooiData8.IsVisible = restoreOooiData9.IsVisible = false;
            FillFuelDetails();
        }

        #endregion

        #region Crews

        #region DinamicHeight

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


        #endregion

        int CrewGridRow = 0;

        public async Task FillCrews()
        {
            var FlightCrewInfo = await svm.GetFlightCrewMembersAsync(flight.flightId);

            lblTotalCrews.Text = "Total Crews: " + FlightCrewInfo.Count().ToString();

            foreach (var crew in FlightCrewInfo)
            {
                CrewGrid.Children.Add(new NewCrewCard(crew, CrewGridRow), 0, 2, CrewGridRow, CrewGridRow + 1);
                CrewGridRow++;
                expanderCrew.ForceUpdateSize();
            }
        }

        async void btnAddCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            await AddNewCrewAsync();
        }

        public async Task AddNewCrewAsync()
        {
            await Task.Delay(1);
            CrewGrid.Children.Add(new NewCrewCard(CrewGridRow), 0, 2, CrewGridRow, CrewGridRow + 1);
            CrewGridRow++;
            expanderCrew.ForceUpdateSize();
        }

        private async Task RemoveCrewGridRow(int cGR)
        {

            var children = CrewGrid.Children.ToList();
            foreach (var child in children.Where(child => Grid.GetRow(child) == cGR))
            {
                CrewGrid.Children.Remove(child);
            }
            await Task.Delay(1);
            children = CrewGrid.Children.ToList();
            if (children.Count < 1)
            {
                CrewGridRow = 0;
            }
        }

        #endregion

        #region Passengers
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


        private async void FillPassengers()
        {
            List<PostFlightPassenger> passengers = await svm.FlightPassengersAsync(flight.flightId);

            List<PassengerTemp> Temppassengers = new List<PassengerTemp>();


            lblTotalPassengers.Text = "Total Passengers    " + passengers.Count.ToString();
            lblOnBoardPassengers.Text = "On Board  " + passengers.Where(x => x.passengerStatusId == 2).Count();

            foreach (var passenger in passengers)
            {
                var tPassengers = new PassengerTemp
                {
                    passengerId = passenger.passengerId,
                    flightId = passenger.flightId,
                    FullName = passenger.firstName + " " + passenger.lastName,
                };

                Temppassengers.Add(tPassengers);
                PassengerRowHeigtht = passengers.Count * 70;
                RaisePropertyChanged(nameof(PassengerRowHeigtht));

                collectionPassenger.ItemsSource = Temppassengers;
            }

        }

        #endregion

        #region SquawksNDiscripencies

        async void btnAddSquawk_Tapped(System.Object sender, System.EventArgs e)
        {
            await AddNewSquawk();
        }

        int SquawkGridRow = 0;

        public async Task AddNewSquawk()
        {
            await Task.Delay(1);
            SquawkGrid.Children.Add(new NewSquawk(SquawkGridRow), 0, 2, SquawkGridRow, SquawkGridRow + 1);
            SquawkGridRow++;
            ExpanderSquawk.ForceUpdateSize();
        }

        #endregion

        #region CheckLists

        void btnCheckList_Tapped(System.Object sender, System.EventArgs e)
        {

        }

        #endregion

        #region SaveNsync

        async void FABSavePostflight_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet(null, "Cancel", null, "Save Only", "Save & Log Flight");
            if (action == "Save Only")
            {
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

                await SaveLocalOooiData();
                await SaveLocalFuelData();


                Services.Settings.HasLocalUpdates = "True";
                MessagingCenter.Send<App>((App)Application.Current, "OnLocalModification");

                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
            }
            else if (action == "Save & Log Flight")
            {

            }
        }

        private async Task SaveLocalFuelData()
        {
            double tempFuelOut = 0;
            double tempFuelOn = 0;
            double tempFuelOff = 0;
            double tempFuelIn = 0;
            double tempFuelBurn = 0;

            if (!String.IsNullOrEmpty(txtFuelOut.Text)) { tempFuelOut = int.Parse(txtFuelOut.Text); }
            if (!String.IsNullOrEmpty(txtFuelIn.Text)) { tempFuelIn = int.Parse(txtFuelIn.Text); }
            if (!String.IsNullOrEmpty(txtFuelOff.Text)) { tempFuelOff = int.Parse(txtFuelOff.Text); }
            if (!String.IsNullOrEmpty(txtFuelOn.Text)) { tempFuelOn = int.Parse(txtFuelOff.Text); }
            if (!String.IsNullOrEmpty(txtFuelBurn.Text)) { tempFuelBurn = int.Parse(txtFuelBurn.Text); }

            PostedFlightFuel fuel = new PostedFlightFuel
            {
                postedFlightId = flight.postedFlightId,
                HasLocalModification = true,
                IsUpdated = false,
                fuelOut = tempFuelOut,
                fuelIn = tempFuelIn,
                fuelOff = tempFuelOff,
                fuelOn = tempFuelOn,
                fuelBurn = tempFuelBurn,
            };

            try
            {
                await svm.InsertUpdateFuelLocal(fuel);
            }
            catch { }
        }

        private async Task SaveLocalOooiData()
        {
            Oooi oooi = new Oooi
            {
                postedFlightId = flight.postedFlightId,
                HasLocalModification = true,
                IsUpdated = false,

                automatedOutTime = DateTime.Parse(pickerDepartDateOooi.Date.ToString("MM/dd/yyyy") + " " + txtOooiOut.Text),
                automatedOffTime = DateTime.Parse(pickerDepartDateOooi.Date.ToString("MM/dd/yyyy") + " " + txtOooiOff.Text),
                automatedOnTime = DateTime.Parse(pickerDepartDateOooi.Date.ToString("MM/dd/yyyy") + " " + txtOooiOn.Text),
                automatedInTime = DateTime.Parse(pickerDepartDateOooi.Date.ToString("MM/dd/yyyy") + " " + txtOooiIn.Text)
            };

            try
            {
                await svm.InsertUpdateOooiLocal(oooi);
            }
            catch { }
        }

        async void btnSync_Tapped(System.Object sender, System.EventArgs e)
        {
            string action = await DisplayActionSheet(null, "Cancel", null, "Push Updates to Server", "Clear and Resync o Server");
            if (action == "Push Updates to Server")
            {



            }
            else if (action == "Clear and Resync o Server")
            {
                MessagingCenter.Send<App>((App)Application.Current, "ClearNdResync");
                logoSynchStatus.TextColor = Color.FromHex("#99E3FF");
                lblSyncStatus.Text = "Synced";
            }


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
        public static List<Tab> Get()
        {
            string color = "#192E48";

            List<Tab> Tablist = new List<Tab>();

            Tablist.Add(new Tab() { TabHeader = "OOOI", TabIndex = 1, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "FUEL", TabIndex = 2, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "CREW", TabIndex = 3, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "PASSENGERS", TabIndex = 4, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "ADDITIONAL DETAILS", TabIndex = 5, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "NOTES", TabIndex = 6, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "EXPENSES", TabIndex = 7, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DOCUMENTS", TabIndex = 8, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DE/ANTI-ICE", TabIndex = 9, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "APU & CUSTOM COMPONENT(S)", TabIndex = 10, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "OILS & FLUIDS", TabIndex = 11, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "SQUAWKS & DISCREPANCIES", TabIndex = 12, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DUTY TIME", TabIndex = 13, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "CHECKLISTS", TabIndex = 14, TabColor = color });

            return Tablist;
        }
    }

    //Issue: https://stackoverflow.com/questions/62416052/how-to-display-contentview-in-a-collectionview-in-xamarin-forms
    public class CrewTempCard
    {
        public string CrewTitle { get; set; }
        public string CrewName { get; set; }
    }

    public class PassengerTemp
    {
        public int passengerId { get; set; }
        public int flightId { get; set; }
        public string FullName { get; set; }
    }

}
