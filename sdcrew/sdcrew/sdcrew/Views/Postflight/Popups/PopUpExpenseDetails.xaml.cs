using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.Postflight.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUpExpenseDetails : PopupPage
    {
        public PopUpExpenseDetails()
        {
            InitializeComponent();
            Loader.IsVisible = false;
        }

        private async void btnClosePopup_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private void FabSaveExpense_Clicked(object sender, EventArgs e)
        {

        }
    }
}