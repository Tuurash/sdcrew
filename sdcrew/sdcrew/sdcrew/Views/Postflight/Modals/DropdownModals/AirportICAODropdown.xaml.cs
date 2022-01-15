using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

using sdcrew.Models;
using sdcrew.Services.Data;
using sdcrew.Views.Preflight;

using Xamarin.Forms;

namespace sdcrew.Views.Postflight.Modals.DropdownModals
{
    public partial class AirportICAODropdown : PopupPage
    {
        string ICAOType = String.Empty;

        PostflightServices postflightServices;

        public AirportICAODropdown()
        {
            InitializeComponent();

            Device.BeginInvokeOnMainThread(async () => await FillAirportData());
        }

        public AirportICAODropdown(string icaoType)
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => await FillAirportData());
            ICAOType = icaoType;
        }

        public async Task FillAirportData()
        {
            postflightServices = new PostflightServices();
            await Task.Delay(0);

            var airports = postflightServices.GetAllAirports();

            List<AllAirports> listArports = new List<AllAirports>();

            foreach (var item in airports)
            {
                var a = new AllAirports
                {
                    airportCode = item.airportCode + " ",
                    airportName = "- " + item.airportName
                };
                listArports.Add(a);
            }

            listIcao.ItemsSource = listArports;
        }

        async void btnColosePopup_Tapped(System.Object sender, System.EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        async void AirportCell_Tapped(System.Object sender, System.EventArgs e)
        {
            var getAirport = (StackLayout)sender;
            var ob = getAirport.BindingContext as AllAirports;

            var code = ob.airportCode;

            if (String.IsNullOrEmpty(ICAOType))
            {
                MessagingCenter.Send(ob, "airportSelectedData");
            }
            else
            {
                MessagingCenter.Send(ob, ICAOType);
            }

            await PopupNavigation.Instance.PopAsync();
        }
    }
}
