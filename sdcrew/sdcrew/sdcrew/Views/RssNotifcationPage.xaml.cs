using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using sdcrew.Repositories;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class RssNotifcationPage : ContentPage
    {
        MiscRepository misc;

        public RssNotifcationPage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(() => sdLoader.IsVisible = true);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () => await FetchAllRss()).ConfigureAwait(false);
        }

        private async Task FetchAllRss()
        {
            misc = new MiscRepository();
            var notifications = await misc.FetchAllRss();

            Device.BeginInvokeOnMainThread(() =>
            {
                RssListView.ItemsSource = notifications;
                sdLoader.IsVisible = false;
            });
        }

        async void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {

            await Task.Delay(300);
            try
            {
                await Navigation.PopToRootAsync(true);
            }
            catch (Exception es)
            {
                throw es;
            }
            //await Shell.Current.GoToAsync("..");
            //await Navigation.PushAsync(new supportPage());
        }

        async void rss_Tapped(System.Object sender, System.EventArgs e)
        {
            dynamic param = ((TappedEventArgs)e).Parameter;
            await Task.Delay(0);

            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = param.HtmlDescription;

            Device.BeginInvokeOnMainThread(() =>
            {
                rssDetailsWebView.Source = htmlSource;
                WebviewHolder.IsVisible = true;
                RssListView.IsVisible = false;
            });
        }

        async void btnCloseWebview_Clicked(System.Object sender, System.EventArgs e)
        {
            await Task.Delay(0);

            Device.BeginInvokeOnMainThread(() =>
            {
                WebviewHolder.IsVisible = false;
                RssListView.IsVisible = true;
            });
        }
    }
}
