using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Services;

using sdcrew.Views.Preflight;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class postFlightPage : ContentPage
    {
        public postFlightPage()
        {
            InitializeComponent();

            if (String.IsNullOrWhiteSpace(Services.Settings.HasLocalUpdates))
            {
                logoSynchStatus.TextColor = Color.FromHex("#99E3FF");
                lblSyncStatus.Text = "Synced";
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
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(1000);

            MessagingCenter.Subscribe<string>(this, "SelectedPostTab", (ob) =>
            {
                if (ob == "LoggedTab")
                {
                    PostflightTabs.SelectedIndex = 1;
                }
                else
                {
                    PostflightTabs.SelectedIndex = 0;
                }
            });
        }


        private async void toolFilter_Clicked(object sender, EventArgs e)
        {
            HapticFeedback.Perform(HapticFeedbackType.Click);
            await PopupNavigation.Instance.PushAsync(new FlightFilterPopup());
        }

        async void btnSynch_Tapped(System.Object sender, System.EventArgs e)
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
    }
}