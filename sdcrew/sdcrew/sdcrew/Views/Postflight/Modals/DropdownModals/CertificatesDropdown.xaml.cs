using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace sdcrew.Views.Postflight.Modals.DropdownModals
{
    public partial class CertificatesDropdown : PopupPage
    {

        public ObservableCollection<string> MyList { get; set; } = new ObservableCollection<string>();

        public CertificatesDropdown()
        {
            InitializeComponent();

            MyList.Add("Item 1");
            MyList.Add("Item 2");
            MyList.Add("Item 3");
            MyList.Add("Item 4");
            MyList.Add("Item 5");
            MyList.Add("Item 6");
            MyList.Add("Item 7");
            BindingContext = this;

        }
    }
}
