using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Views.Postflight.Modals.DropdownModals;
using Xamarin.Forms;

namespace sdcrew.Views.Postflight.Modals
{
    public partial class AddDocuments : PopupPage
    {
        public AddDocuments()
        {
            InitializeComponent();
        }

        void btnColosePopup_Tapped(System.Object sender, System.EventArgs e)
        {
        }

        void btnDocumentType_Tapped(System.Object sender, System.EventArgs e)
        {
            Task.Run(() => PopupNavigation.Instance.PushAsync(new CertificatesDropdown()));
        }
    }
}
