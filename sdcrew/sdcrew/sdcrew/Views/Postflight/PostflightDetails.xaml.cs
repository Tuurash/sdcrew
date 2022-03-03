using ChoETL;

using Microsoft.AppCenter.Crashes;

using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.Views.Postflight.Modals.DropdownModals;
using sdcrew.Views.Postflight.Popups;
using sdcrew.Views.Postflight.SubViews;
using sdcrew.Views.Settings;
using sdcrew.Views.ViewHelpers.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class PostflightDetails : ContentPage, INotifyPropertyChanged
    {

        int GeneratedPostedFlightId = 0;

        public List<Tab> AllTabs { get; set; }
        dynamic flight = new PostFlightVM();

        string Block_Time = String.Empty;
        string Flight_Time = String.Empty;

        //Oooi TempVals
        int OooiArrivalAirportId = 0;
        int OooiDepartAirportId = 0;

        PostflightServices svm;

        bool IsRunning = false;
        string SubNavItemString = ""; //Users/brotecs/Desktop/Misc/temp.json

        string CallbackPage = String.Empty;

        public PostflightDetails(dynamic flightObj, string callBackFrom)
        {
            InitializeComponent();
            BindingContext = this;
            CallbackPage = callBackFrom;

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

            FillOooiDetails();
            FillFuelDetails();
            FillCrews();
            FillPassengers();
            FillAdditionalDetails();
            FillOilsNFluids();
            FillDeAntiIce();

            #region messagingcenters

            MessagingCenter.Subscribe<AllAirports>(this, "airportSelectedData", (ob) =>
            {
                AllAirports receivedData = ob;
            });

            MessagingCenter.Subscribe<AllAirports>(this, "OooiDeparture", (ob) =>
            {
                AllAirports receivedData = ob;
                dropdownDepartOooi.Text = receivedData.airportCode;
                OooiDepartAirportId = ob.airportId;
            });

            MessagingCenter.Subscribe<AllAirports>(this, "OooiArrive", (ob) =>
            {
                AllAirports receivedData = ob;
                dropdownArriveOooi.Text = receivedData.airportCode;
                OooiArrivalAirportId = ob.airportId;
            });

            //Crew
            MessagingCenter.Subscribe<string>(this, "CrewRemoved", async (ob) =>
             {
                 int CGR = int.Parse(ob);
                 await RemoveCrewGridRow(CGR);
                 expanderCrew.ForceUpdateSize();
             });

            MessagingCenter.Subscribe<App>((App)Application.Current, "CrewUpdated", (sender) =>
            {
                expanderCrew.ForceUpdateSize();
            });

            //Squawk
            MessagingCenter.Subscribe<App>((App)Application.Current, "SquawkUpdated", (sender) =>
            {
                ExpanderSquawk.ForceUpdateSize();
            });

            MessagingCenter.Subscribe<string>(this, "SquawkRemoved", async (ob) =>
            {
                int SGR = int.Parse(ob);
                await RemoveSquawkGridRow(SGR);
                ExpanderSquawk.ForceUpdateSize();
            });

            //Expense N Doc
            MessagingCenter.Subscribe<string>(this, "ExpenseRemoved", async (ob) =>
            {
                int EGR = int.Parse(ob);
                await RemoveExpenseGridRow(EGR);
                expanderExpense.ForceUpdateSize();
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "ExpenseAdded", async (sender) =>
            {
                await AddNewExpense();
            });

            //Doc
            MessagingCenter.Subscribe<string>(this, "DocRemoved", async (ob) =>
            {
                int DGR = int.Parse(ob);
                await RemoveDocGridRow(DGR);
                expanderDocuments.ForceUpdateSize();
            });

            MessagingCenter.Subscribe<TempDoc>(this, "DocAdded", async (ob) =>
            {
                TempDoc savedDoc = ob;
                await AddNewDoc(savedDoc);
                expanderDocuments.IsExpanded = true;
            });

            //Duty Time
            MessagingCenter.Subscribe<CrewDetailsVM>(this, "DutyTimeSelected", async (ob) =>
            {
                await Task.Delay(100);
                CrewDetailsVM crewDetails = ob;
                await AddNewDutyTime(crewDetails);
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "DutyUpdated", (sender) =>
            {
                expanderDutyTime.ForceUpdateSize();
            });

            #region dropdownPostbacks

            MessagingCenter.Subscribe<DropdownObj>(this, "BusinessCatagoryPicked", (ob) =>
            {
                lblBCatagory.Text = dropdownBCatagory.Text = ob.ObjName;
                selectedBCategoryId = ob.ObjId;
            });

            MessagingCenter.Subscribe<DropdownObj>(this, "DelaytypePicked", (ob) =>
            {
                lblDelayType.Text = dropdownDelayType.Text = ob.ObjName;
                selectedDelayTypeId = ob.ObjId;
            });

            MessagingCenter.Subscribe<DropdownObj>(this, "DepartmentPicked", (ob) =>
            {
                lblChargeTo.Text = dropdownChargeTo.Text = ob.ObjName;
                selectedDepartmentId = ob.ObjId;

            });

            MessagingCenter.Subscribe<DropdownObj>(this, "mixTypeChanged", (ob) =>
            {
                dropdownMixRatio.Text = lblMixRatioDAIce.Text = dropdownChargeTo.Text = ob.ObjName;
                selectedDeIceMixRatioTypeId = ob.ObjId;
            });

            MessagingCenter.Subscribe<DropdownObj>(this, "DAIceTypeChanged", (ob) =>
            {
                dropdownTypeDAIce.Text = lblTypeDAIce.Text = dropdownChargeTo.Text = ob.ObjName;
                selectedDeIceType = ob.ObjId;
            });

            MessagingCenter.Subscribe<CrewDetailsVM>(this, "CrewSelected", async (ob) =>
             {
                 CrewDetailsVM crewDetails = ob;
                 await AddNewCrewAsync(crewDetails);
             });

            #endregion

            #endregion
        }

        protected override async void OnAppearing()
        {
            //Should follow this https://stackoverflow.com/questions/67763091/xamarin-animation-causing-my-application-to-skip-many-frames
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false);

            try
            {
                MessagingCenter.Subscribe<App>((App)Application.Current, "OnChecklistChanged", (sender) =>
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await CheckAllChecklistSStatus();
                    });
                });
            }
            catch { }

            await Task.Run(async () =>
            {
                await svm.AddPreflight_Checklists(flight.tripId, 2);
                await CheckAllChecklistSStatus();
            }).ConfigureAwait(false);
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
            try
            {

                foreach (var tab in AllTabs)
                {
                    var Frame = this.FindByName<Frame>(tab.TabName);
                    Device.BeginInvokeOnMainThread(() => Frame.IsVisible = true);
                }
                Device.BeginInvokeOnMainThread(() => collectionViewListHorizontal.ItemsSource = AllTabs);
            }
            catch (Exception exc)
            {

                Crashes.TrackError(exc);
            }
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

            string p2 = param.ToString().Trim(' ', '/', '&', '-', '(', ')');
            string p3 = p2.Replace(" ", "");
            string p4 = p3.Replace(@"&", "");
            p4 = p4.Replace(@"-", "");
            p4 = p4.Replace(@"(", "");
            p4 = p4.Replace(@")", "");
            p4 = p4.Replace(@"/", "");
            p4 = p4.Replace(" ", "");

            var FrameName = this.FindByName<Frame>(p4);
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

        //Oooi Completed with issues
        #region Oooi

        private void FillOooiDetails()
        {
            try
            {
                //HeaderFields
                lblDepartOooi.Text = dropdownDepartOooi.Text = flight.departureIcao;
                lblArriveOooi.Text = dropdownArriveOooi.Text = flight.airportIcao;

                OooiDepartAirportId = flight.departureAirportId;
                OooiArrivalAirportId = flight.arrivalAirportId;

                Oooi oooiDetails = svm.GetOooiDetails(flight.postedFlightId);

                //apuComponents
                Task.Run(async () =>
                {
                    int pFlightID = 0;

                    if (oooiDetails != null)
                    {
                        pFlightID = flight.postedFlightId;
                    }
                    ApuNCustomComponents apuNCustomComponents = await svm.FetchApuNComponents(pFlightID, flight.aircraftProfileId, DateTime.Parse(flight.StartDate));
                    FillApuNCustomControls(apuNCustomComponents);
                }).ConfigureAwait(false);

                if (oooiDetails != null)
                {
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

                }
                else
                {
                    txtOooiOut.Text = txtOooiIn.Text = txtOooiOff.Text = txtOooiOn.Text = "";
                    lblOooiOut.Text = lblOooiIn.Text = lblOooiOff.Text = lblOooiOn.Text = "N/A";
                    lblOooiBlockTime.Text = lblTaxiTime.Text = lblOooiOnInTaxi.Text = lblOooiFlightTime.Text = "";
                }

                if (flight.StartDate != "N/A" & flight.EndDate != "N/A")
                {
                    pickerDepartDateOooi.Date = DateTime.Parse(flight.StartDate);
                    pickerArriveDateOooi.Date = DateTime.Parse(flight.EndDate);
                }

                restoreOooiData1.IsVisible = restoreOooiData2.IsVisible = restoreOooiData3.IsVisible = restoreOooiData4.IsVisible = false;


            }
            catch { }
        }

        int OooiChangeCount = 0;

        async void OooiTextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            var txtbox = sender as Entry;
            await Task.Delay(10);
            OooiChangeCount++;
            if (OooiChangeCount > 4)
            {
                txtOooiIn.TextColor = txtOooiOff.TextColor = txtOooiOn.TextColor = txtOooiOut.TextColor = lblOooiIn.TextColor = lblOooiOff.TextColor = lblOooiOn.TextColor = lblOooiOut.TextColor = Color.White;
                restoreOooiData1.IsVisible = restoreOooiData2.IsVisible = restoreOooiData3.IsVisible = restoreOooiData4.IsVisible = true;
            }

            //Changed Values todo:Time Input Check
            lblOooiOut.Text = txtOooiOut.Text;
            lblOooiIn.Text = txtOooiIn.Text;
            lblOooiOff.Text = txtOooiOff.Text;
            lblOooiOn.Text = txtOooiOn.Text;


            if (DateTime.TryParse(txtOooiOut.Text, out DateTime TempOut) == true && DateTime.TryParse(txtOooiIn.Text, out DateTime TempIn) == true
                && DateTime.TryParse(txtOooiOff.Text, out DateTime TempOff) == true && DateTime.TryParse(txtOooiOn.Text, out DateTime TempOn) == true)
            {


                var genBlockTime = (TempIn - TempOut).Duration();
                lblOooiBlockTime.Text = genBlockTime.ToString(@"hh\:mm") + "/" + ((genBlockTime.TotalMinutes / 60)).ToString("0.0");



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

        public async Task PostOooiAsync()
        {
            //On=> Flight Stop ; Off=>Flight Start  ; In=> blockStart ; Out=> blockstop
            if (flight.postedFlightId != 0)
            {
                //Put method to Update
                OooiPostModel oooiPostModel = new OooiPostModel
                {
                    PostedFlightId = flight.postedFlightId,
                    AircraftProfileId = flight.aircraftProfileId,
                    ArrivalAirportId = OooiArrivalAirportId,
                    DepartureAirportId = OooiDepartAirportId,
                    BlockStartDateTime = DateTime.Parse(lblOooiIn.Text),
                    BlockStopDateTime = DateTime.Parse(lblOooiOut.Text),
                    FlightStartDateTime = DateTime.Parse(lblOooiOff.Text),
                    FlightStopDateTime = DateTime.Parse(lblOooiOn.Text),
                    ScheduledFlightId = flight.scheduledFlightId,
                };

                var UpdateResponse = await svm.PutOooiAsync(oooiPostModel, oooiPostModel.PostedFlightId);
            }
            else
            {
                //Post and will return PostedFlightId
                OooiPostModel oooiPostModel = new OooiPostModel
                {
                    AircraftProfileId = flight.aircraftProfileId,
                    ArrivalAirportId = OooiArrivalAirportId,
                    DepartureAirportId = OooiDepartAirportId,
                    BlockStartDateTime = DateTime.Parse(lblOooiIn.Text),
                    BlockStopDateTime = DateTime.Parse(lblOooiOut.Text),
                    FlightStartDateTime = DateTime.Parse(lblOooiOff.Text),
                    FlightStopDateTime = DateTime.Parse(lblOooiOn.Text),
                    ScheduledFlightId = flight.scheduledFlightId,
                };

                GeneratedPostedFlightId = await svm.PostOooiAsync(oooiPostModel);
            }
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


        public async Task PutFuelAsync()
        {
            if (CurrentPostedFlightId != 0)
            {
                //Put method to Save
                FuelPostingModel model = new FuelPostingModel
                {
                    PostedFlightId = CurrentPostedFlightId,
                    FuelOut = int.Parse(txtFuelOut.Text ?? "0"),
                    FuelOff = int.Parse(txtFuelOff.Text ?? "0"),
                    FuelOn = int.Parse(txtFuelOn.Text ?? "0"),
                    FuelIn = int.Parse(txtFuelIn.Text ?? "0"),
                    FuelBurn = int.Parse(lblFuelBurn.Text ?? "0"),
                    //QuantityTypeId
                    //PlannedUp 
                    //actualup
                    //UpliftQuantityTypeId
                };

                var UpdateResponse = await svm.PutFuelAsync(model, CurrentPostedFlightId);
            }
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
            Block_Time = lblOooiBlockTime.Text;
            Flight_Time = lblOooiFlightTime.Text;

            var FlightCrewInfo = await svm.GetFlightCrewMembersAsync(flight.flightId);

            lblTotalCrews.Text = "Total Crews: " + FlightCrewInfo.Count().ToString();

            foreach (var crew in FlightCrewInfo)
            {
                CrewGrid.Children.Add(new NewCrewCard(crew, CrewGridRow, Block_Time, Flight_Time), 0, 2, CrewGridRow, CrewGridRow + 1);
                CrewGridRow++;
                expanderCrew.ForceUpdateSize();
            }
        }

        async void btnAddCrew_Tapped(System.Object sender, System.EventArgs e)
        {
            var crews = await svm.GetCrewsAsync();
            await PopupNavigation.Instance.PushAsync(new CrewListDropdown("CrewSelected"));
        }

        public async Task AddNewCrewAsync(CrewDetailsVM crew)
        {
            Block_Time = lblOooiBlockTime.Text;
            Flight_Time = lblOooiFlightTime.Text;
            await Task.Delay(1);
            CrewGrid.Children.Add(new NewCrewCard(crew, CrewGridRow, Block_Time, Flight_Time), 0, 2, CrewGridRow, CrewGridRow + 1);
            CrewGridRow++;
            expanderCrew.ForceUpdateSize();
            expanderCrew.IsExpanded = true;
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

        #region PostedflghtAdditional Nd Notes

        private async void FillAdditionalDetails()
        {
            PostedFlightAdditional additionalDetails = await svm.PostedFlightAdditionalAsync(flight.postedFlightId);
            if (additionalDetails != null)
            {
                if (additionalDetails.businessCategoryId != 0)
                {
                    lblBCatagory.Text = dropdownBCatagory.Text = svm.GetBusinessCatagoryAsync(additionalDetails.businessCategoryId).Result.Name;
                }
                else
                {
                    var bCatagory = await svm.GetBusinessCatagoriesAsync();
                    dropdownBCatagory.Text = bCatagory.Select(x => x.Name).FirstOrDefault();
                }
                if (additionalDetails.delayTypeId != 0)
                {
                    lblDelayType.Text = dropdownDelayType.Text = svm.GetDelayTypeAsync(additionalDetails.delayTypeId).Result.Name;
                }
                else
                {
                    var dType = await svm.GetDelayTypesAsync();
                    dropdownDelayType.Text = dType.Select(x => x.Name).FirstOrDefault();
                }
                if (additionalDetails.departmentId != 0)
                {
                    lblChargeTo.Text = dropdownChargeTo.Text = svm.GetDepartmentAsync(additionalDetails.departmentId).Result.Name;
                }
                else
                {
                    var dept = await svm.GetDepartmentsAsync();
                    dropdownChargeTo.Text = dept.Select(x => x.Name).FirstOrDefault();
                }

                lblDelayDuration.Text = txtDelayDuration.Text = additionalDetails.delayDuration.ToString();
                lblGoArounds.Text = txtGoArounds.Text = additionalDetails.goArounds.ToString();
                lblRejectedTakeoffs.Text = txtRejectedTakeoffs.Text = additionalDetails.rejectedTakeoffs.ToString();
            }
        }

        async void dropdownBCatagory_Tapped(System.Object sender, System.EventArgs e)
        {
            var businessTypes = await svm.GetBusinessCatagoriesAsync();

            List<DropdownObj> bTypes = new List<DropdownObj>();
            foreach (var item in businessTypes)
            {
                var obj = new DropdownObj
                {
                    ObjId = item.Id,
                    ObjName = item.Name
                };

                bTypes.Add(obj);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new DropdownModal(bTypes, "BusinessCatagoryPicked")));
        }

        async void dropdownDelayType_Tapped(System.Object sender, System.EventArgs e)
        {
            var delayTypes = await svm.GetDelayTypesAsync();

            List<DropdownObj> dTypes = new List<DropdownObj>();
            foreach (var item in delayTypes)
            {
                var obj = new DropdownObj
                {
                    ObjId = item.Id,
                    ObjName = item.Name
                };

                dTypes.Add(obj);
            }

            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new DropdownModal(dTypes, "DelaytypePicked")));
        }

        async void dropdownChargeTo_Tapped(System.Object sender, System.EventArgs e)
        {
            var departments = await svm.GetDepartmentsAsync();

            List<DropdownObj> ChargeTos = new List<DropdownObj>();
            foreach (var item in departments)
            {
                var obj = new DropdownObj
                {
                    ObjId = item.Id,
                    ObjName = item.Name
                };

                ChargeTos.Add(obj);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new DropdownModal(ChargeTos, "DepartmentPicked")));
        }

        async void txtAdditional_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            await Task.Delay(10);

            lblDelayDuration.Text = txtDelayDuration.Text;
            lblGoArounds.Text = txtGoArounds.Text;
            lblRejectedTakeoffs.Text = txtRejectedTakeoffs.Text;
        }

        int selectedBCategoryId = 300;
        int selectedDelayTypeId = 147;
        int selectedDepartmentId = 283;

        public async Task PutAdditionalAsync()
        {
            string lNote = "";

            if (!String.IsNullOrEmpty(txtNote.Text))
            {
                lNote = txtNote.Text;
            }
            else { lNote = ""; }

            if (CurrentPostedFlightId != 0)
            {
                //Put method to Save
                AdditionalPostingModel model = new AdditionalPostingModel
                {
                    PostedFlightId = CurrentPostedFlightId,
                    DelayTypeId = selectedDelayTypeId,
                    DelayDuration = int.Parse(txtDelayDuration.Text),
                    GoArounds = int.Parse(txtGoArounds.Text),
                    RejectedTakeoffs = int.Parse(txtRejectedTakeoffs.Text),
                    LegNotes = lNote,
                    DepartmentId = selectedDepartmentId,
                    BusinessCategoryId = selectedBCategoryId,
                };

                var UpdateResponse = await svm.PutAdditionalAsync(model, model.PostedFlightId);
            }
        }

        #endregion

        #region Expenses

        int ExpenseGridRow = 0;

        private async void btnAddExpense_Tapped(object sender, EventArgs e)
        {
            //await AddNewExpense();
            await PopupNavigation.Instance.PushAsync(new PopUpExpenseDetails());
        }

        public async Task AddNewExpense()
        {
            await Task.Delay(1000);
            ExpenseGrid.Children.Add(new NewExpense(ExpenseGridRow), 0, 2, ExpenseGridRow, ExpenseGridRow + 1);
            ExpenseGridRow++;
            expanderExpense.ForceUpdateSize();
        }

        private async Task RemoveExpenseGridRow(int cGR)
        {

            var children = ExpenseGrid.Children.ToList();
            foreach (var child in children.Where(child => Grid.GetRow(child) == cGR))
            {
                ExpenseGrid.Children.Remove(child);
            }
            await Task.Delay(1);
            children = ExpenseGrid.Children.ToList();
            if (children.Count < 1)
            {
                ExpenseGridRow = 0;
            }
        }

        #endregion

        #region Documents

        int DocGridRow = 0;

        private async void btnAddDoc_Tapped(object sender, EventArgs e)
        {
            //await AddNewDoc();
            await PopupNavigation.Instance.PushAsync(new PopUpDocumentDetails());
        }

        public async Task AddNewDoc(TempDoc doc)
        {
            await Task.Delay(1000);
            DocumentGrid.Children.Add(new NewDoc(DocGridRow, doc), 0, 2, DocGridRow, DocGridRow + 1);
            DocGridRow++;
            expanderDocuments.ForceUpdateSize();
        }

        private async Task RemoveDocGridRow(int cGR)
        {

            var children = DocumentGrid.Children.ToList();
            foreach (var child in children.Where(child => Grid.GetRow(child) == cGR))
            {
                DocumentGrid.Children.Remove(child);
            }
            await Task.Delay(1);
            children = DocumentGrid.Children.ToList();
            if (children.Count < 1)
            {
                DocGridRow = 0;
            }
        }


        #endregion

        #region DeAnti_Ice

        private async void FillDeAntiIce()
        {
            //oooiDetails.automatedOutTime.ToString("HH:mm") ?? "00:00"; 
            var deIce = await svm.PostedFlightDeIceAsync(flight.postedFlightId);
            if (deIce != null)
            {
                if (deIce.deIceStartDateTime != DateTime.MinValue)
                {
                    lblStartDAIce.Text = deIce.deIceStartDateTime.ToString("dd MMM yyyy");
                    lblEndDAIce.Text = deIce.deIceEndDateTime.ToString("dd MMM yyyy");

                    pickerStartDateDAIce.Date = deIce.deIceStartDateTime;
                    pickerEndDateDAIce.Date = deIce.deIceEndDateTime;

                    txtStartTimeDAIce.Text = deIce.deIceStartDateTime.ToString("HH:mm") ?? "00:00";
                    txtEndTimeDAIce.Text = deIce.deIceEndDateTime.ToString("HH:mm") ?? "00:00";
                }

                var mixRatios = await svm.MixTypesAsync();
                if (deIce.deIceMixRatioTypeId == 0)
                {

                    lblMixRatioDAIce.Text = dropdownMixRatio.Text = mixRatios.FirstOrDefault().name;
                }
                else
                {
                    lblMixRatioDAIce.Text = dropdownMixRatio.Text = mixRatios.FirstOrDefault(x => x.id == deIce.deIceMixRatioTypeId).name;
                }

                if (deIce.deIceTypeId == 1) { lblTypeDAIce.Text = dropdownTypeDAIce.Text = "I"; }
                else if (deIce.deIceTypeId == 2) { lblTypeDAIce.Text = dropdownTypeDAIce.Text = "II"; }
                else if (deIce.deIceTypeId == 3) { lblTypeDAIce.Text = dropdownTypeDAIce.Text = "III"; }
                else if (deIce.deIceTypeId == 4) { lblTypeDAIce.Text = dropdownTypeDAIce.Text = "IV"; }
                else { dropdownTypeDAIce.Text = "I"; }
            }
        }

        private void pickerDAIce_DateSelected(object sender, DateChangedEventArgs e)
        {
            lblStartDAIce.Text = pickerStartDateDAIce.Date.ToString("dd MMM yyyy");
            lblEndDAIce.Text = pickerEndDateDAIce.Date.ToString("dd MMM yyyy");
        }

        private async void dropdownMixRatio_Tapped(object sender, EventArgs e)
        {
            var mixTypes = await svm.MixTypesAsync();

            List<DropdownObj> mxTypes = new List<DropdownObj>();
            foreach (var item in mixTypes)
            {
                var obj = new DropdownObj
                {
                    ObjId = item.id,
                    ObjName = item.name
                };
                mxTypes.Add(obj);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new DropdownModal(mxTypes, "mixTypeChanged")));
        }

        private void dropdownTypeDAIce_Tapped(object sender, EventArgs e)
        {
            List<DropdownObj> DAIceType = new List<DropdownObj>();
            DAIceType.Add(new DropdownObj { ObjId = 1, ObjName = "I" });
            DAIceType.Add(new DropdownObj { ObjId = 2, ObjName = "II" });
            DAIceType.Add(new DropdownObj { ObjId = 3, ObjName = "III" });
            DAIceType.Add(new DropdownObj { ObjId = 4, ObjName = "IV" });

            PopupNavigation.Instance.PushAsync(new DropdownModal(DAIceType, "DAIceTypeChanged"));
        }

        int selectedDeIceMixRatioTypeId = 1;
        int selectedDeIceType = 1;

        public async Task PutDeIceAsync()
        {
            if (CurrentPostedFlightId != 0)
            {
                //Put method to Save
                DeIcePostingModel model = new DeIcePostingModel
                {
                    DeIceStartDateTime = pickerStartDateDAIce.Date,
                    DeIceEndDateTime = pickerEndDateDAIce.Date,
                    DeIceMixRatioTypeId = selectedDeIceMixRatioTypeId,
                    DeIceTypeId = selectedDeIceType
                };

                var UpdateResponse = await svm.PutDeIceAsync(model, CurrentPostedFlightId);
            }
        }

        #endregion

        #region Apus & CustomComponents

        bool ApuComponentChanged = false;

        private void FillApuNCustomControls(ApuNCustomComponents apuNCustomComponents)
        {

            if (apuNCustomComponents != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    lblApuName.Text = lblApuName2.Text = apuNCustomComponents.Name;
                    lblCarryOver.Text = (apuNCustomComponents.CarryOverMinutes / 60).ToString() + " HR";

                    txtCarryOver.Text = (apuNCustomComponents.CarryOverMinutes / 60).ToString();

                    lblCurrent.Text = txtCurrent.Text = (apuNCustomComponents.CurrentMinutes / 60).ToString();
                    lblDuration.Text = txtDuration.Text = (int.Parse(txtCurrent.Text) - int.Parse(txtCarryOver.Text)).ToString();
                });
            }

        }

        async void APuComponents_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //var txtbox = sender as Entry;
            await Task.Delay(10);
            if (!String.IsNullOrEmpty(txtCurrent.Text) & !String.IsNullOrEmpty(txtCarryOver.Text))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (int.Parse(txtCurrent.Text) >= int.Parse(txtCarryOver.Text))
                    {
                        txtCarryOver.TextColor = txtCurrent.TextColor = Color.White;

                        lblApuName.Text = lblApuName2.Text;
                        lblCarryOver.Text = txtCarryOver.Text + " HR";
                        lblCurrent.Text = txtCurrent.Text;
                        lblDuration.Text = txtDuration.Text = (int.Parse(txtCurrent.Text) - int.Parse(txtCarryOver.Text)).ToString();
                    }
                    else
                    {
                        txtCarryOver.TextColor = txtCurrent.TextColor = Color.Red;
                        lblDuration.Text = txtDuration.Text = (int.Parse(txtCurrent.Text) - int.Parse(txtCarryOver.Text)).ToString();
                    }

                    if (txtCycle.Text != "")
                    {
                        lblCycles.Text = txtCycle.Text;
                    }

                });
            }
        }


        #endregion

        #region OilsnFluids

        private async void FillOilsNFluids()
        {
            var fluids = await svm.GetFluidsAsync();

            if (fluids != null)
            {
                if (fluids.Count != 0)
                {
                    lblFluidType1.Text = lblFluid1Body.Text = fluids.Select(x => x.type).First();
                    if (fluids.Select(x => x.usage).First() != 0)
                    {
                        lblFuid1Usage.Text = txtFuid1Usage.Text = fluids.Select(x => x.usage).First().ToString();
                    }

                    if (fluids.Select(x => x.type).Skip(1).First() != null)
                    {
                        lblFluidType1.Text = lblFluid1Body.Text = fluids.Select(x => x.type).Skip(1).First();
                        if (fluids.Select(x => x.usage).Skip(1).First() != 0)
                        {
                            lblFuid1Usage.Text = txtFuid1Usage.Text = fluids.Select(x => x.usage).Skip(1).First().ToString();
                        }
                    }

                    if (fluids.Select(x => x.type).Skip(2).First() != null)
                    {
                        lblFluidType1.Text = lblFluid1Body.Text = fluids.Select(x => x.type).Skip(2).First();
                        if (fluids.Select(x => x.usage).Skip(2).First() != 0)
                        {
                            lblFuid1Usage.Text = txtFuid1Usage.Text = fluids.Select(x => x.usage).Skip(2).First().ToString();
                        }
                    }
                }
            }
        }

        void txtFuidUsage_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            lblFuid1Usage.Text = txtFuid1Usage.Text;
            lblFluid2Usage.Text = txtfluid2Usage.Text;
            lblFluid3Usage.Text = txtfluid3Usage.Text;
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
            ExpanderSquawk.IsExpanded = true;
        }

        private async Task RemoveSquawkGridRow(int cGR)
        {

            var children = SquawkGrid.Children.ToList();
            foreach (var child in children.Where(child => Grid.GetRow(child) == cGR))
            {
                SquawkGrid.Children.Remove(child);
            }
            await Task.Delay(1);
            children = SquawkGrid.Children.ToList();
            if (children.Count < 1)
            {
                SquawkGridRow = 0;
            }
        }

        #endregion

        #region DutyTime

        private async void btnAddDutyTime_Tapped(object sender, EventArgs e)
        {
            //var crews = await svm.GetCrewsAsync();
            await PopupNavigation.Instance.PushAsync(new CrewListDropdown("DutyTimeSelected"));

            //await AddNewDutyTime();
        }

        int DutyTimeGridRow = 0;

        public async Task AddNewDutyTime(CrewDetailsVM crewDetails)
        {
            await Task.Delay(1000);
            DutyTimeGrid.Children.Add(new NewDutyTime(DutyTimeGridRow, crewDetails), 0, 2, DutyTimeGridRow, DutyTimeGridRow + 1);
            DutyTimeGridRow++;
            expanderDutyTime.ForceUpdateSize();
            expanderDutyTime.IsExpanded = true;
        }

        #endregion

        #region CheckLists

        async void btnCheckList_Tapped(System.Object sender, System.EventArgs e)
        {
            if (IsRunning == false)
            {
                IsRunning = true;

                HapticFeedback.Perform(HapticFeedbackType.Click);
                Device.BeginInvokeOnMainThread(() =>
                {
                    pfDetailControls.IsEnabled = false;
                    Loader.IsVisible = true;

                });
                await Task.Delay(1);

                await Task.Run(() => PopupNavigation.Instance.PushAsync(new Checklist(flight.tripId)))
                    .ContinueWith(x =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            pfDetailControls.IsEnabled = true;
                            Loader.IsVisible = false;

                        });
                    })
                    .ConfigureAwait(false);

                await Task.Run(() => IsRunning = false);
            }
        }


        private async Task CheckAllChecklistSStatus()
        {
            MainThread.BeginInvokeOnMainThread(() => lblChecklistUpdatingStatus.IsVisible = true);

            await Task.WhenAll(FillPilotStatus(), FillMaintenaceStatus(), FillSchedulingStatus())
                .ContinueWith(x =>
                {
                    MainThread.BeginInvokeOnMainThread(() => lblChecklistUpdatingStatus.IsVisible = false);
                });
        }

        private async Task FillPilotStatus()
        {
            //Scheduling, Maintenance, Pilot
            var pilotStatus = await svm.CheckStatusChecklist(flight, "Pilot");

            // &#xf11a; => \uf11a  \uE80d;  "\uE80f;  \uE80e;
            if (pilotStatus == 3)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblpilotStatus.Text = "N/A";
                    lblpilotStatus.FontSize = 14;
                    lblpilotStatus.TextColor = Color.White;
                });
            }
            else
            {
                lblpilotStatus.FontSize = 22;

                if (pilotStatus == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        lblpilotStatus.Text = "\uE80d";
                        lblpilotStatus.TextColor = Color.FromHex("FF646A");
                    });
                }
                else
                {
                    if (pilotStatus == 1)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            lblpilotStatus.Text = "\uE80e"; //uE80e
                            lblpilotStatus.TextColor = Color.FromHex("33E850");
                        });
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            lblpilotStatus.Text = "\uE80f";
                            lblpilotStatus.TextColor = Color.FromHex("FFFF6C");
                        });
                    }
                }
            }

        }

        private async Task FillMaintenaceStatus()
        {
            //Scheduling, Maintenance, Pilot

            var MaintenanceStatus = await svm.CheckStatusChecklist(flight, "Maintenance");

            // &#xf11a; => \uf11a  \uE80d;  "\uE80f;  \uE80e;
            if (MaintenanceStatus == 3)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblMaintenanceStatus.Text = "N/A";
                    lblMaintenanceStatus.FontSize = 14;
                    lblMaintenanceStatus.TextColor = Color.White;
                });
            }
            else
            {
                if (MaintenanceStatus == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        lblMaintenanceStatus.Text = "\uE80d";
                        lblMaintenanceStatus.TextColor = Color.FromHex("FF646A");
                    });

                }
                else
                {
                    if (MaintenanceStatus == 1)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            lblMaintenanceStatus.Text = "\uE80e";
                            lblMaintenanceStatus.TextColor = Color.FromHex("33E850");
                        });

                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            lblMaintenanceStatus.Text = "\uE80f";
                            lblMaintenanceStatus.TextColor = Color.FromHex("FFFF6C");
                        });
                    }
                }
            }
        }

        private async Task FillSchedulingStatus()
        {

            var SchedulingStatus = await svm.CheckStatusChecklist(flight, "Scheduling");

            // &#xf11a; => \uf11a  \uE80d;  "\uE80f;  \uE80e;
            if (SchedulingStatus == 3)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblSchedulingStatus.Text = "N/A";
                    lblSchedulingStatus.FontSize = 14;
                    lblSchedulingStatus.TextColor = Color.White;
                });
            }
            else
            {
                if (SchedulingStatus == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        lblSchedulingStatus.Text = "\uE80d";
                        lblSchedulingStatus.TextColor = Color.FromHex("FF646A");
                    });


                }
                else
                {
                    if (SchedulingStatus == 1)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            lblSchedulingStatus.Text = "\uE80e";
                            lblSchedulingStatus.TextColor = Color.FromHex("33E850");
                        });


                    }
                    else
                    {

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            lblSchedulingStatus.Text = "\uE80f";
                            lblSchedulingStatus.TextColor = Color.FromHex("FFFF6C");
                        });

                    }
                }
            }
        }

        #endregion

        #region SaveNsync

        int CurrentPostedFlightId = 0;

        async void FABSavePostflight_Clicked(System.Object sender, System.EventArgs e)
        {
            string action = String.Empty;
            if (!String.IsNullOrEmpty(CallbackPage))
            {
                if (CallbackPage == "Logged")
                {
                    action = await DisplayActionSheet(null, "Cancel", null, "ESign", "Save Only");
                }
                else if (CallbackPage == "NotLogged")
                {
                    action = await DisplayActionSheet(null, "Cancel", null, "Save Only", "Save & Log Flight");
                }
            }

            if (action == "Save Only")
            {
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

                await SaveLocallyAll();

                Services.Settings.HasLocalUpdates = "True";
                MessagingCenter.Send<App>((App)Application.Current, "OnLocalModification");

                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
            }
            else if (action == "Save & Log Flight")
            {
                await PostOooiAsync();
                await Task.Delay(10);
                await PutFuelAsync();

                await PutAdditionalAsync();
                await PutDeIceAsync();

                await SaveLocallyAll();

                if (flight.postedFlightId != 0)
                {
                    CurrentPostedFlightId = flight.postedFlightId;
                }
                else
                {
                    CurrentPostedFlightId = GeneratedPostedFlightId;
                }
            }
            else if (action == "ESign")
            {

            }
        }

        private async Task SaveLocallyAll()
        {
            await SaveLocalOooiData();
            await SaveLocalFuelData();
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

    #region Helpers

    public class Tab
    {
        public string TabHeader { get; set; }
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        public string TabColor { get; set; }
    }

    public class Tabs
    {
        public static List<Tab> Get()
        {
            string color = "#192E48";

            List<Tab> Tablist = new List<Tab>();

            Tablist.Add(new Tab() { TabHeader = "OOOI", TabName = "OOOI", TabIndex = 1, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "FUEL", TabName = "FUEL", TabIndex = 2, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "CREW", TabName = "CREW", TabIndex = 3, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "PASSENGERS", TabName = "PASSENGERS", TabIndex = 4, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "ADDITIONAL DETAILS", TabName = "ADDITIONALDETAILS", TabIndex = 5, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "NOTES", TabName = "NOTES", TabIndex = 6, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "EXPENSES", TabName = "EXPENSES", TabIndex = 7, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DOCUMENTS", TabName = "DOCUMENTS", TabIndex = 8, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DE/ANTI-ICE", TabName = "DEANTIICE", TabIndex = 9, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "APU & CUSTOM COMPONENT(S)", TabName = "APUCUSTOMCOMPONENTS", TabIndex = 10, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "OILS & FLUIDS", TabName = "OILSFLUIDS", TabIndex = 11, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "SQUAWKS & DISCREPANCIES", TabName = "SQUAWKSDISCREPANCIES", TabIndex = 12, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "DUTY TIME", TabName = "DUTYTIME", TabIndex = 13, TabColor = color });
            Tablist.Add(new Tab() { TabHeader = "CHECKLISTS", TabName = "CHECKLISTS", TabIndex = 14, TabColor = color });

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

    public class DropdownObj
    {
        public int ObjId { get; set; }
        public string ObjName { get; set; }
    }

    #endregion
}
