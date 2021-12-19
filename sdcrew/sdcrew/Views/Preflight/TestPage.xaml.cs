using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Views.Postflight.Modals;
using sdcrew.Views.Postflight.Modals.DropdownModals;
using sdcrew.Views.Postflight.SubViews;
using sdcrew.Views.Settings;
using Xamarin.Forms;

namespace sdcrew.Views.Preflight
{
    public partial class TestPage : ContentPage
    {
        int GridRow = 1;

        public TestPage()
        {
            InitializeComponent();

            
        }

        
        

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {

            var pickerList = new List<string>();
            pickerList.Add("1 Minuite");
            pickerList.Add("5 Minuite");
            pickerList.Add("10 Minuite");
            pickerList.Add("15 Minuite");

            Task.Run(() => PopupNavigation.Instance.PushAsync(new popupDropdown(pickerList,"RefreshtimePicked")));
        }




    }

}
