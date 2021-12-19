using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using sdcrew.Repositories;

using Xamarin.Forms;

namespace sdcrew.Views
{
    public partial class RssNotifcationPage : ContentPage
    {
        MiscRepository misc;

        public RssNotifcationPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
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

        void btnBack_Tapped(System.Object sender, System.EventArgs e)
        {
        }

        void rss_Tapped(System.Object sender, System.EventArgs e)
        {
            dynamic param = ((TappedEventArgs)e).Parameter;

            //HtmlDescription  e.SelectedItem as Job

            var browser = new WebView();
            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = param.HtmlDescription;


        }
    }
}
