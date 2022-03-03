using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using sdcrew.ViewModels.Preflight;
using sdcrew.Views.Login;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class AllPreflights : ContentView, INotifyPropertyChanged
    {
        AllPreFlightsViewModel viewModel;

        private ObservableCollection<PreflightGroup> _allGroups;
        private ObservableCollection<PreflightGroup> _expandedGroups;
        private readonly PreflightServices _preflightServices;

        credentialsViewModel credentials = new credentialsViewModel();
        UserServices userServices = new UserServices();

        string tempTimeZone = "";

        private bool IsRefreshing = false;

        public AllPreflights()
        {
            _preflightServices = new PreflightServices();
            viewModel = new AllPreFlightsViewModel();

            RefreshTime = Services.Settings.GetRefreshTime;

            InitializeComponent();
            BindingContext = this;

            Task.Delay(1000);

            Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loader.IsVisible = true;
                    lblRefreshTime.Text = Services.Settings.GetRefreshTime; //RefreshTime
                });


                await PopulateData();

                tempTimeZone = Services.Settings.TimeZone;

                int tokenFlag = await credentials.PerformRefreshToken();
                if (tokenFlag == 1)
                {
                    await PerformBackgroundRefresh().ConfigureAwait(false);
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => App.Current.MainPage = new loginPage());
                }
            });



            #region MessagingCenter

            try
            {
                MessagingCenter.Subscribe<string>(this, "PreflightDateUpdated", async (ob) =>
                {
                    await PerformBackgroundRefresh().ConfigureAwait(false);
                });

                MessagingCenter.Subscribe<string, string>("MyApp", "TimeZoneChanged", async (sender, arg) =>
                {
                    if (tempTimeZone == Services.Settings.TimeZone)
                    {
                        MessagingCenter.Send<App>((App)Application.Current, "TimeZoneUpdatedAndRefreshed");
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = false);
                        tempTimeZone = Services.Settings.TimeZone;
                        Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                        await PerformRefresh();
                    }
                });

                MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected", async (sender) =>
                {

                    Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                    await PopulateData();
                });
            }
            catch (Exception) { }

            #endregion
        }

        int refreshFlag = 0;
        private async Task PopulateData()
        {

            _allGroups = viewModel.getAllPreflightsList();
            if (_allGroups.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
                await UpdateListContent();
            }
            else
            {
                if (refreshFlag > 0 || String.IsNullOrEmpty(Services.Settings.FilterItems) == false)
                {
                    await UpdateListContent();
                    Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false);
                }
                else
                {
                    await PerformRefresh();
                }
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = _expandedGroups.IndexOf(((PreflightGroup)((ImageButton)sender).CommandParameter));
            _allGroups[selectedIndex].Expanded = !_allGroups[selectedIndex].Expanded;
            try
            {
                await UpdateListContent();
            }
            catch (Exception) { }
        }

        private async Task UpdateListContent()
        {
            IsRefreshing = true;
            await Task.Delay(1);
            _expandedGroups = new ObservableCollection<PreflightGroup>();

            foreach (PreflightGroup group in _allGroups)
            {
                //Creating new Groups so we do not alter original list
                PreflightGroup newGroup = new PreflightGroup(group.Date, group.Expanded);

                if (group.Expanded)
                {
                    foreach (PreflightVM flight in group)
                    {
                        newGroup.Add(flight);
                    }
                }
                _expandedGroups.Add(newGroup);
            }
            //GroupedView.ItemsSource=null;

            Device.BeginInvokeOnMainThread(() =>
            {
                GroupedView.ItemsSource = _expandedGroups;
                Loader.IsVisible = false;
            });

            GroupedView.IsRefreshing = false;
            IsRefreshing = false;
        }

        #region Refresh

        private string refresh = Services.Settings.GetRefreshTime;

        public string RefreshTime
        {
            get
            {
                return refresh;
            }
            set
            {
                refresh = value;
                OnPropertyChanged(nameof(refresh));
            }
        }

        const int RefreshDuration = 2;

        private async void GroupedView_Refreshing(object sender, EventArgs e)
        {
            await PerformRefresh();
        }

        public async Task PerformRefresh()
        {
            if (IsRefreshing == false)
            {
                IsRefreshing = true;

                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = false);

                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await _preflightServices.InsertDatas();

                await Task.Run(() =>
                {
                    MessagingCenter.Send<App>((App)Application.Current, "TimeZoneUpdatedAndRefreshed");
                    Task.Delay(10);
                });


                await PopulateData();
                RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
                Services.Settings.GetRefreshTime = RefreshTime;

                Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
                GroupedView.EndRefresh();

                IsRefreshing = false;

                await Task.Delay(2);
                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);

                refreshFlag++;
            }
        }


        public async Task PerformBackgroundRefresh()
        {
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await _preflightServices.InsertDatas();

            await Task.Run(() =>
            {
                MessagingCenter.Send<App>((App)Application.Current, "TimeZoneUpdatedAndRefreshed");
                Task.Delay(10);
            });


            await PopulateData();
            RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
            Services.Settings.GetRefreshTime = RefreshTime;

            Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
            IsRefreshing = false;

            await Task.Delay(2);
            Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);
        }


        #endregion

        private void FlightGetDetails_Tapped(object sender, EventArgs e)
        {
            if (IsRefreshing == false)
            {

                IsRefreshing = true;
                try
                {
                    dynamic flightObj = ((TappedEventArgs)e).Parameter;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Loader.IsVisible = true;

                        if (flightObj.eventName == "Aircraft")
                        {
                            await Navigation.PushAsync(new PreflightDetails(flightObj))
                             .ContinueWith(x =>
                             { Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false); });
                        }
                        else
                        {
                            string StaffNotes = "";

                            if (flightObj.staffEventId != 0 & flightObj.staffEventId != null)
                            {
                                StaffNotes = await _preflightServices.GetStaffNotes(flightObj.staffEventId);
                            }

                            await Navigation.PushAsync(new EventDetails(flightObj, StaffNotes))
                             .ContinueWith(x =>
                             {
                                 Device.BeginInvokeOnMainThread(() =>
                               {
                                   Loader.IsVisible = false;
                               });
                             });
                        }
                    });

                }
                catch { }

                IsRefreshing = false;
            }
        }

        void GetNoteDetails_Tapped(System.Object sender, System.EventArgs e)
        {
            if (IsRefreshing == false)
            {
                var getNoteID = ((TappedEventArgs)e).Parameter;
                Task.Run(() => PopupNavigation.Instance.PushAsync(new NotesDetails(int.Parse(getNoteID.ToString()))));
            }
        }

    }
}

