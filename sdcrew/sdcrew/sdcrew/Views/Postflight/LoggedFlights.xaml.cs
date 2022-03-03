using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using sdcrew.ViewModels.Postflight;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class LoggedFlights : ContentView
    {
        credentialsViewModel credentials = new credentialsViewModel();
        PostflightServices postflightServices = new PostflightServices();

        LoggedViewModel loggedViewmodel;

        string tempTimeZone = "";
        bool IsRefreshing = false;
        int refreshFlag = 0;

        public LoggedFlights()
        {
            loggedViewmodel = new LoggedViewModel();

            InitializeComponent();

            BindingContext = this;

            Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loader.IsVisible = true;
                    lblRefreshTime.Text = Services.Settings.GetRefreshTime; //RefreshTime
                });

                await PopulateData();
            });

            MessagingCenter.Subscribe<string>(this, "PostflightUpdateAll", async (ob) =>
            {
                await PopulateData();
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected", async (sender) =>
            {

                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                await PopulateData();
            });
            ////Insert or refresh nothing is called in Logged
            //MessagingCenter.Subscribe<App>((App)Application.Current, "ClearNdResync", async (sender) =>
            //{
            //    await ClearAndReSyncFlights();
            //});
        }

        ObservableCollection<PostFlightVM> LoggedList = new ObservableCollection<PostFlightVM>();

        private async Task PopulateData()
        {
            LoggedList = loggedViewmodel.getLogged_Postflights();
            await Task.Delay(1);

            if (LoggedList.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loader.IsVisible = false;
                    LoggedListView.ItemsSource = LoggedList;
                });
            }
            else
            {
                if (refreshFlag > 0 || String.IsNullOrEmpty(Services.Settings.FilterItems) == false)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Loader.IsVisible = false;
                        LoggedListView.ItemsSource = LoggedList;
                    });
                }
                else
                {
                    await PerformRefresh();
                }
            }
        }

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

                        await Navigation.PushAsync(new PostflightDetails(flightObj, "Logged"))
                             .ContinueWith(x =>
                             { Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false); });
                    });
                }
                catch { }

                IsRefreshing = false;
            }
        }

        #region Refreshing

        const int RefreshDuration = 2;
        public string RefreshTime = "";

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
                //await postflightServices.InsertDatas();
                await PopulateData();

                RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
                Services.Settings.GetRefreshTime = RefreshTime;

                Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
                LoggedListView.EndRefresh();

                IsRefreshing = false;

                await Task.Delay(2);
                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);

                refreshFlag++;
            }
        }

        #endregion
    }
}
