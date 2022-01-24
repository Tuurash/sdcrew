using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight.Modals.DropdownModals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Crews : PopupPage
    {
        PostflightServices postflightServices;

        public Crews()
        {
            InitializeComponent();
        }

        public Crews(string callbackFrom)
        {
            InitializeComponent();
            FillCrews();
            //Device.BeginInvokeOnMainThread(async () => await FillCrews());
        }

        private async Task FillCrews()
        {
            List<CrewDetailsVM> crews = new List<CrewDetailsVM>();
            try
            {
                crews = await postflightServices.GetCrewsAsync();
            }
            catch (Exception exc)
            {
                Crashes.TrackError(exc);
            }


            listCrews.ItemsSource = crews;
        }

        private async void btnClosePopup_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private void Crew_Tapped(object sender, EventArgs e)
        {

        }
    }
}