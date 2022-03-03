using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.Views.Postflight.Modals.DropdownModals;
using sdcrew.Views.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace sdcrew.Views.Postflight.SubViews
{
    public partial class NewApproach : ContentView
    {
        PostflightServices postflightServices;
        public NewApproach()
        {
            InitializeComponent();
            postflightServices = new PostflightServices();
        }

        int ParentGridRow = 0;

        public NewApproach(int ApprchGridRow)
        {
            InitializeComponent();
            ParentGridRow = ApprchGridRow;
            postflightServices = new PostflightServices();

            MessagingCenter.Subscribe<AllAirports>(this, "ApproachICAOSelected", (ob) =>
            {
                AllAirports receivedData = ob;
                dropDownApprchICAO.Text = receivedData.airportCode;
            });

            MessagingCenter.Subscribe<string>(this, "ApproachTypeSelected", (ob) =>
            {
                dropDownApprchType.Text = ob;
            });
        }

        void dropDownApprchICAO_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new AirportICAODropdown("ApproachICAOSelected")));
        }

        void isEVS_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {

        }

        async void dropDownApprchType_Tapped(System.Object sender, System.EventArgs e)
        {
            var approachTypes = await postflightServices.ApproachTypesAsync();
            List<string> Names = new List<string>();
            foreach (var item in approachTypes)
            {
                Names.Add(item.Name);
            }
            await Task.Run(async () => await PopupNavigation.Instance.PushAsync(new popupDropdown(Names, "ApproachTypeSelected")));
        }

        private void dlt_Tapped(object sender, EventArgs e)
        {
            MessagingCenter.Send(ParentGridRow.ToString(), "ApproachRemoved");
        }
    }
}
