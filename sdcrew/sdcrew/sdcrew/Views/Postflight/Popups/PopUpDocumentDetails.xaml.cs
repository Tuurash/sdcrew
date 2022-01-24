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
    public partial class PopUpDocumentDetails : PopupPage
    {
        public PopUpDocumentDetails()
        {
            InitializeComponent();
        }

        private async void btnClose_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        private void FabSaveDoc_Clicked(object sender, EventArgs e)
        {

        }
    }
}