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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

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
    }
}