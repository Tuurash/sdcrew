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
using Xamarin.Essentials;
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

        public AllPreflights()
        {
            _preflightServices = new PreflightServices();
            viewModel = new AllPreFlightsViewModel();

            RefreshTime = Services.Settings.GetRefreshTime;

            InitializeComponent();
            BindingContext = this;
        }

        public async void PageAppearing()
        {
            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            await PopulateData();

            int tokenFlag = await credentials.PerformRefreshToken();
            if (tokenFlag == 1)
            {
                lblRefreshTime.Text = RefreshTime;
                tempTimeZone = Services.Settings.TimeZone;

                await PerformRefresh().ConfigureAwait(false);

                MessagingCenter.Subscribe<string, string>("MyApp", "TimeZoneChanged", async (sender, arg) =>
                {
                    if (tempTimeZone == Services.Settings.TimeZone) { }
                    else
                    {
                        Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                        await PerformRefresh();
                    }

                    //PopulateData();
                });
            }
            else
            {
                App.Current.MainPage = new loginPage();
            }

            await Task.Delay(1);

            #region MessagingCenter

            try
            {
                MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected",async (sender) =>
                {
                    
                    Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                    await Task.Delay(1);
                    //await PerformRefresh();
                    await PopulateData();
                });
            }
            catch (Exception) { }

            #endregion
        }

        private async Task PopulateData()
        {
            _allGroups = viewModel.getAllPreflightsList();
            if(_allGroups.Count>0)
            {
                await UpdateListContent();
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
            await Task.Delay(10);
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
            await _preflightServices.InsertDatas();

            await PopulateData();

            RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
            
            Services.Settings.GetRefreshTime = RefreshTime;
 
            Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);

            GroupedView.EndRefresh();
        }

        #endregion

        private async void FlightGetDetails_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            dynamic flightObj = ((TappedEventArgs)e).Parameter;

            if (flightObj.eventName == "Aircraft")
            {
                await Navigation.PushAsync(new PreflightDetails(flightObj));
            }
            else
            {
                await Navigation.PushAsync(new EventDetails(flightObj));
            }

            Loader.IsVisible = false;
        }

        void GetNoteDetails_Tapped(System.Object sender, System.EventArgs e)
        {
            var getNoteID = ((TappedEventArgs)e).Parameter;
            Task.Run(() => PopupNavigation.Instance.PushAsync(new NotesDetails(int.Parse(getNoteID.ToString()))));
        }

    }

}

