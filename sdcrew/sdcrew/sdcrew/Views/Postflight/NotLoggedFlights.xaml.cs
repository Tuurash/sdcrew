﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.ViewModels;
using sdcrew.ViewModels.Postflight;
using sdcrew.Views.Login;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class NotLoggedFlights : ContentView
    {
        credentialsViewModel credentials = new credentialsViewModel();
        PostflightServices postflightServices = new PostflightServices();

        NotLoggedFlightsViewModel NotLoggedViewmodel;

        string tempTimeZone = "";
        bool IsRefreshing = false;

        public NotLoggedFlights()
        {


            InitializeComponent();
            BindingContext = this;
            Task.Delay(1000);
            NotLoggedViewmodel = new NotLoggedFlightsViewModel();
            Task.Run(async () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loader.IsVisible = true;
                    lblRefreshTime.Text = Services.Settings.GetRefreshTime; //RefreshTime
                });

                await PopulateData();

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

            MessagingCenter.Subscribe<string>(this, "PostflightUpdated", async (ob) =>
            {
                await PerformRefresh();
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnFilterSelected", async (sender) =>
            {
                Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);
                await PopulateData();
            });

            MessagingCenter.Subscribe<App>((App)Application.Current, "ClearNdResync", async (sender) =>
            {
                await ClearAndReSyncFlights();
            });

        }

        int refreshFlag = 0;

        List<PostFlightVM> notLoggedList = new List<PostFlightVM>();

        private async Task PopulateData()
        {
            notLoggedList = NotLoggedViewmodel.getNotLogged_Postflights().OrderByDescending(x=>x.flightStartDateTime).ToList();
            await Task.Delay(1);

            if (notLoggedList.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    NotLoggedListView.ItemsSource = null;
                    Task.Delay(1);
                    NotLoggedListView.ItemsSource = notLoggedList;
                    Loader.IsVisible = false;
                });
            }
            else
            {
                if (refreshFlag > 0 || String.IsNullOrEmpty(Services.Settings.FilterItems) == false)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        NotLoggedListView.ItemsSource = null;
                        Loader.IsVisible = false;
                        NotLoggedListView.ItemsSource = notLoggedList;
                    });
                }
                else
                {
                    await PerformInsertion();
                }
            }
        }

        const int RefreshDuration = 2;
        public string RefreshTime = "";

        private async void GroupedView_Refreshing(object sender, EventArgs e)
        {
            await PerformRefresh();
        }
        public async Task ClearAndReSyncFlights()
        {
            if (IsRefreshing == false)
            {
                IsRefreshing = true;

                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = false);

                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await postflightServices.InsertDatas();

                await PopulateData();

                RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
                Services.Settings.GetRefreshTime = RefreshTime;

                Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
                NotLoggedListView.EndRefresh();

                IsRefreshing = false;

                await Task.Delay(2);
                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);

                refreshFlag++;
            }
        }

        public async Task PerformInsertion()
        {
            if (IsRefreshing == false)
            {
                IsRefreshing = true;

                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = false);

                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await postflightServices.InsertDatas();

                await PopulateData();

                RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
                Services.Settings.GetRefreshTime = RefreshTime;

                Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
                NotLoggedListView.EndRefresh();

                IsRefreshing = false;

                await Task.Delay(2);
                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);

                refreshFlag++;
            }
        }


        public async Task PerformRefresh()
        {
            if (IsRefreshing == false)
            {
                IsRefreshing = true;

                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = false);

                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await postflightServices.RefreshDatas();

                await PopulateData();

                RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
                Services.Settings.GetRefreshTime = RefreshTime;

                Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
                NotLoggedListView.EndRefresh();

                IsRefreshing = false;

                await Task.Delay(2);
                Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);

                refreshFlag++;
            }
        }

        public async Task PerformBackgroundRefresh()
        {
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await postflightServices.RefreshDatas();

            MessagingCenter.Send("DummyVal", "PostflightUpdateAll");

            await PopulateData();
            RefreshTime = DateTime.Now.ToString("dd MMM yyyy - hh:mm");
            Services.Settings.GetRefreshTime = RefreshTime;

            Device.BeginInvokeOnMainThread(() => lblRefreshTime.Text = RefreshTime);
            IsRefreshing = false;

            await Task.Delay(2);
            Device.BeginInvokeOnMainThread(() => AContentControl.IsEnabled = true);
        }

        void FlightGetDetails_Tapped(System.Object sender, System.EventArgs e)
        {
            Loader.IsVisible = true;
            Task.Delay(100);
            dynamic flightObj = ((TappedEventArgs)e).Parameter;

            Navigation.PushAsync(new PostflightDetails(flightObj, "NotLogged"))
                 .ContinueWith(x =>
                 { Device.BeginInvokeOnMainThread(() => Loader.IsVisible = false); });
        }

    }
}
