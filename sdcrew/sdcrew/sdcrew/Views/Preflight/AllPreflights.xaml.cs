using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.ViewModels.Preflight;
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

        public AllPreflights()
        {
            _preflightServices = new PreflightServices();
            viewModel = new AllPreFlightsViewModel();

            RefreshTime = Services.Settings.GetRefreshTime;

            InitializeComponent();
            BindingContext = this;

            
            lblRefreshTime.Text = RefreshTime;
        }

        public void PageAppearing()
        {
            PopulateData();

            #region MessagingCenter
            try
            {
                Task.Run(() =>
                {
                    MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected", (sender) =>
                    {
                        PopulateData();
                    });

                    MessagingCenter.Subscribe<string, string>("MyApp", "TimeZoneChanged", (sender, arg) =>
                    {
                        PopulateData();
                    });
                });
            }
            catch (Exception) { }
            #endregion
        }

        private void PopulateData()
        {
            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            _allGroups = viewModel.getAllPreflightsList();

            if (_allGroups.Count == 0)
            {
                MainThread.BeginInvokeOnMainThread(async () => await PerformRefresh());
            }

            UpdateListContent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = _expandedGroups.IndexOf(((PreflightGroup)((ImageButton)sender).CommandParameter));
            _allGroups[selectedIndex].Expanded = !_allGroups[selectedIndex].Expanded;
            try
            {
                UpdateListContent();
            }
            catch (Exception) { }

        }

        private void UpdateListContent()
        {
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

            GroupedView.ItemsSource = _expandedGroups;

            GroupedView.IsRefreshing = false;

            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false);

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
            PopulateData();
      
            RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
            lblRefreshTime.Text = RefreshTime;
            Services.Settings.GetRefreshTime = RefreshTime;
            OnPropertyChanged(nameof(RefreshTime));

            GroupedView.EndRefresh();
        }

        #endregion

        private async void FlightGetDetails_Tapped(object sender, EventArgs e)
        {
            Loader.IsVisible = true;
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

