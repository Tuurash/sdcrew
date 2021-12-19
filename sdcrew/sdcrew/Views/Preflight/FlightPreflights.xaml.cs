using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using sdcrew.ViewModels.Preflight;
using sdcrew.Views.Login;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class FlightPreflights : ContentView
    {
        FlightPreflightsViewModel viewModel;

        private ObservableCollection<PreflightGroup> _allGroups;
        private ObservableCollection<PreflightGroup> _expandedGroups;

        credentialsViewModel credentials = new credentialsViewModel();
        UserServices userServices = new UserServices();

        public FlightPreflights()
        {
            InitializeComponent();
            BindingContext = viewModel = new FlightPreflightsViewModel();

            Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loader.IsVisible = true;
                    lblRefreshTime.Text = Services.Settings.GetRefreshTime; //RefreshTime
                });

                await PopulateData();
            });

            #region MessagingCenter

            MessagingCenter.Subscribe<string, string>("MyApp", "TimeZoneChanged", (sender, arg) =>
            {
                Device.BeginInvokeOnMainThread(() => FContentControl.IsEnabled = false);
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            });

            MessagingCenter.Subscribe<string, string>("MyApp", "TimeZoneUpdatedAndRefreshed", async (sender, arg) =>
            {
                await PerformRefresh();
            });

            try
            {
                MessagingCenter.Subscribe<App>((App)Application.Current, "TimeZoneUpdatedAndRefreshed", async (sender) =>
                {
                    Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                    await PerformRefresh();
                });

                MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected", async (sender) =>
                {
                    Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                    await Task.Delay(1);
                    await PopulateData();
                });
            }
            catch (Exception) { }

            #endregion
        }

        int refreshFlag = 0;

        private async Task PopulateData()
        {
            _allGroups = viewModel.getAllAircraftEventsList();
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

            Device.BeginInvokeOnMainThread(() =>
            {
                GroupedView.ItemsSource = _expandedGroups;
                Loader.IsVisible = false;
            });

            GroupedView.IsRefreshing = false;
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
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await PopulateData();

            RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
            Services.Settings.GetRefreshTime = RefreshTime;

            Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
            GroupedView.EndRefresh();

            await Task.Delay(2);
            Device.BeginInvokeOnMainThread(() =>
            {
                FContentControl.IsEnabled = true;
                Loader.IsVisible = false;
            });

            refreshFlag++;
        }

        #endregion

        private void FlightGetDetails_Tapped(object sender, EventArgs e)
        {
            dynamic flightObj = ((TappedEventArgs)e).Parameter;

            Device.BeginInvokeOnMainThread(async () =>
            {
                Loader.IsVisible = true;

                await Navigation.PushAsync(new PreflightDetails(flightObj))
                     .ContinueWith(x =>
                     { Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false); });
            });
        }

        void GetNoteDetails_Tapped(System.Object sender, System.EventArgs e)
        {
            var getNoteID = ((TappedEventArgs)e).Parameter;
            Task.Run(() => PopupNavigation.Instance.PushAsync(new NotesDetails(int.Parse(getNoteID.ToString()))));
        }
    }

}