using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Services.Data;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.Modals.DropdownModals
{
    public partial class CrewListDropdown : PopupPage
    {

        PostflightServices postflightServices;
        string CallBackFrm = String.Empty;

        public CrewListDropdown()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => await FillCrews());
        }

        public CrewListDropdown(string callbackFrom)
        {
            InitializeComponent();
            CallBackFrm = callbackFrom;

            Device.BeginInvokeOnMainThread(async () => await FillCrews());
        }

        private async Task FillCrews()
        {
            postflightServices = new PostflightServices();

            var crews = await postflightServices.GetCrewsAsync();
            listCrews.ItemsSource = crews;
        }

        void CrewCell_Tapped(System.Object sender, System.EventArgs e)
        {
            var getCrewDetails = (Label)sender;
            var ob = getCrewDetails.BindingContext as CrewDetailsVM;

            if (String.IsNullOrEmpty(CallBackFrm))
            {

            }
            else
            {
                MessagingCenter.Send(ob, CallBackFrm);
            }

            PopupNavigation.Instance.PopAsync();
        }

        void btnClosePopup_Tapped(System.Object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
