using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Views.Postflight;
using Xamarin.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace sdcrew.Views.ViewHelpers.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DropdownModal : PopupPage, INotifyPropertyChanged
    {
        string messagingCentermsg = "";

        public DropdownModal(List<string> pickerList, string callbackParentMNote)
        {
            InitializeComponent();

            messagingCentermsg = callbackParentMNote;

            RowHeigtht = pickerList.Count * 10;
            RaisePropertyChanged(nameof(RowHeigtht));

            listPopup.ItemsSource = pickerList;
        }

        public DropdownModal(List<DropdownObj> pickerList, string callbackParentMNote)
        {
            InitializeComponent();
            messagingCentermsg = callbackParentMNote;

            RowHeigtht = pickerList.Count * 10;
            RaisePropertyChanged(nameof(RowHeigtht));

            listPopup.ItemsSource = pickerList;
        }

        async void itm_Tapped(System.Object sender, System.EventArgs e)
        {
            var getItem = (Label)sender;
            var ob = getItem.BindingContext as DropdownObj;

            var selectedItem = ob;

            MessagingCenter.Send(ob, messagingCentermsg);

            await PopupNavigation.Instance.PopAsync();
        }


        //Dinamic Height
        private int rowHeigtht;
        public int RowHeigtht
        {
            get { return rowHeigtht; }
            set
            {
                rowHeigtht = value;
                RaisePropertyChanged(nameof(rowHeigtht));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}