using Plugin.XamarinFormsSaveOpenPDFPackage;

using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Repositories.PreflightRepos;
using sdcrew.Services.Data;
using sdcrew.ViewModels.Preflight;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class PreflightDetails : ContentPage, INotifyPropertyChanged
    {
        public List<Tab> AllTabs { get; set; }


        dynamic flight = new PreflightVM();

        private PreflightDetailsViewModel preflightViewModel;
        PreflightServices svm;

        PreflightRepository preflightRepository = new PreflightRepository();

        bool IsRunning = false;


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

            if (!String.IsNullOrEmpty(Services.Settings.PreflightSubviewVisibility))
            {
                stackSubNav.IsVisible = bool.Parse(Services.Settings.PreflightSubviewVisibility);
            }
            else
            {
                stackSubNav.IsVisible = true;
            }



            MessagingCenter.Subscribe<string>(this, "PreflightSubviewUpdated", (ob) =>
             {
                 stackSubNav.IsVisible = bool.Parse(Services.Settings.PreflightSubviewVisibility);
             });

            FillCrews();
            FillPassengers();
            FillFlightInfo();
            FillAirport();

            lstCrewIDs.Clear();
            lstBusinessPersonIds.Clear();


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
                await svm.AddPreflight_Checklists(flight.scheduledAircraftTripId, flight.legNumber, 1, flight.scheduledFlightId);
                await CheckAllChecklistSStatus();
                await svm.AddServices_Preflight(flight.scheduledAircraftTripId);
            }).ConfigureAwait(false);
        }

        #region Checklist Status


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

        public List<int> lstCrewIDs = new List<int>();
        public List<int> lstBusinessPersonIds = new List<int>();

        #region HeaderSubNav

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


        #endregion

        public void FillFlightInfo()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lblAircraftTail.Text = flight.tailNumber;
                lblFlightTripID.Text = flight.TripID;
                lblDepartureICAO.Text = flight.departureAirportIcao;
                lblArrivalICAO.Text = flight.arrivalAirportIcao;

                lblDepartHeader.Text = flight.departureAirportIcao;
                lblArrivalHeader.Text = flight.arrivalAirportIcao;

                lblDepartureDate.Text = flight.StartTime.Date.ToString("dd MMM yyyy");
                lblArrivalDate.Text = flight.EndTime.Date.ToString("dd MMM yyyy");

                lblDepartureTime.Text = flight.StartTime.ToString("H:mm") + " " + Services.Settings.TimeZone;
                lblArrivalTime.Text = flight.EndTime.ToString("H:mm") + " " + Services.Settings.TimeZone;

                lblETE.Text = flight.ete;

                iconFlight.TextColor = Color.FromHex(flight.color);
                lblIconHeader.TextColor = Color.FromHex(flight.color);
            });
        }

        public void FillCrews()
        {
            FlightCrewMember crewMember = new FlightCrewMember();

            List<FlightCrewMember> Crews = new List<FlightCrewMember>();

            foreach (var crew in flight.crews)
            {
                crewMember = new FlightCrewMember
                {
                    id = crew.id,
                    crewMemberType = crew.crewMemberType,
                    fullName = crew.fullName,
                    fullPhoneNumber = crew.fullPhoneNumber,
                    CallVisible = crew.CallVisible
                };

                Crews.Add(crewMember);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                CrewRowHeigtht = Crews.Count * 73;
                RaisePropertyChanged(nameof(CrewRowHeigtht));

                lblCrewCount.Text = Crews.Count.ToString();
                CollectionCrew.ItemsSource = Crews;
            });

            foreach (var item in Crews)
            {
                lstCrewIDs.Add(item.id);
            }
        }

        public void FillPassengers()
        {
            FlightPassenger flightPassenger = new FlightPassenger();
            List<FlightPassenger> passengers = new List<FlightPassenger>();

            string getCallVisible = "false";

            foreach (var passenger in flight.flightpassengers)
            {
                if (passenger.passengerPhoneNumber != "" & passenger.passengerPhoneNumber != null)
                {
                    getCallVisible = "true";
                }
                else { getCallVisible = "false"; }

                flightPassenger = new FlightPassenger
                {
                    PassengerName = passenger.PassengerName,
                    businessPersonId = passenger.businessPersonId,
                    passengerPhoneNumber = passenger.passengerPhoneNumber,
                    pCallVisible = getCallVisible
                };

                passengers.Add(flightPassenger);
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                PassengerRowHeigtht = passengers.Count * 160;
                RaisePropertyChanged(nameof(PassengerRowHeigtht));

                lblPassengerCount.Text = passengers.Count.ToString();
                lblPassengerOnboardCount.Text = flight.PassengerOnboardCount;
            });

            CollectionPassenger.ItemsSource = passengers;

            foreach (var item in passengers)
            {
                lstBusinessPersonIds.Add(item.businessPersonId);
            }
        }

        public void FillAirport()
        {
            //await Task.Delay(1);

            Device.BeginInvokeOnMainThread(() =>
            {
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
            });
        }

        private async void btnCheckList_Tapped(object sender, EventArgs e)
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

                await Task.Run(() => PopupNavigation.Instance.PushAsync(new FlightCheckList(flight.scheduledAircraftTripId, flight.legNumber, flight.scheduledFlightId)))
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

        private async void btnServices_Tapped(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            var ServiceType = ((TappedEventArgs)e).Parameter;
            await Task.Run(() => PopupNavigation.Instance.PushAsync(new FlightServices(flight.scheduledAircraftTripId, ServiceType.ToString())));
        }

        private async void btnBack_Tapped(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            await Navigation.PopAsync(true);
        }

        private async void FABItenery_Clicked(object sender, EventArgs e)
        {
            if (IsRunning == false)
            {
                IsRunning = true;

                string getIteneraryName = flight.tailNumber + "_" + flight.scheduledAircraftTripId + "_" + flight.legNumber;

                string getType = await DisplayActionSheet(null, "Cancel", null, "View Crew Itinerary", "View Pax Itinerary");

                int type = 1;

                await Task.Delay(1);

                if (getType == "View Crew Itinerary")
                {
                    type = 1;

                }
                else if (getType == "View Pax Itinerary") { type = 2; }
                else
                {
                    IsRunning = false;
                    MainThread.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() => Device.BeginInvokeOnMainThread(() =>
                {
                    pfDetailControls.IsEnabled = false;
                    Loader.IsVisible = true;

                }));

                ArrayList CrewIDarrayList = new ArrayList(lstCrewIDs);

                ArrayList BusinessPersonIdarrayList = new ArrayList(lstBusinessPersonIds);

                int legNumber = flight.legNumber;

                var pdf = await Task.Run(async () => await preflightRepository.Fetch_Itineray(type, legNumber, CrewIDarrayList, BusinessPersonIdarrayList, flight.scheduledAircraftTripId, getIteneraryName));

                if (pdf != null)
                {
                    Device.BeginInvokeOnMainThread(async () => await CrossXamarinFormsSaveOpenPDFPackage.Current.SaveAndView(getIteneraryName + ".pdf", "application/pdf", pdf, PDFOpenContext.ChooseApp));
                }

                await Task.Run(() => IsRunning = false)
                    .ContinueWith(x =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
   {
       pfDetailControls.IsEnabled = true;
       Loader.IsVisible = false;
   });
                    });
            }
        }

        #region Call&Mails

        public async Task MakeCall(string PhoneNumber)
        {
            string number = PhoneNumber;
            await Task.Delay(10);
            MainThread.BeginInvokeOnMainThread(() => PhoneDialer.Open(number));
        }

        public async Task OpenInMail(string MailAddress)
        {
            var address = MailAddress;

            await Task.Delay(10);
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

        void passengerMakeCall_Tapped(System.Object sender, System.EventArgs e)
        {

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