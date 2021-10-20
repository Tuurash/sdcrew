using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CheckBox = Plugin.InputKit.Shared.Controls.CheckBox;
using sdcrew.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlightFilterPopup : PopupPage
    {
        private readonly PreflightServices preflightServices;

        filteredTailNumber filteredTail = new filteredTailNumber();

        public FlightFilterPopup()
        {
            preflightServices = new PreflightServices();
            InitializeComponent();
            this.BindingContext = this;

        }

        public List<filteredTailNumber> FilteredTailNumbers { get => getTailNumbers(); }

        public List<filteredTailNumber> getTailNumbers()
        {

            var filteredTails = preflightServices.getTailnumbers();

            List<filteredTailNumber> TailList = new List<filteredTailNumber>();


            if (Services.Settings.FilterItems != null && Services.Settings.FilterItems != "")
            {
                string fi = Services.Settings.FilterItems.Remove(0, 1);
                string[] filterItemsArray = fi.Split(',');

                foreach (var item in filteredTails)
                {
                    bool has = filterItemsArray.Any(x => x.Contains(item));
                    if (has == true)
                    {
                        TailList.Add(new filteredTailNumber
                        {
                            TailNumber = item,
                            isChecked = false
                        });
                    }
                    else
                    {
                        TailList.Add(new filteredTailNumber
                        {
                            TailNumber = item,
                            isChecked = true
                        });
                    }
                }
            }
            else
            {
                foreach (var item in filteredTails)
                {
                    TailList.Add(new filteredTailNumber
                    {
                        TailNumber = item,
                        isChecked = true
                    });
                }
            }

            var TailCollection = new List<filteredTailNumber>(TailList);
            return TailCollection;
        }


        string filtertails = "";

        private void CheckBox_CheckChanged(object sender, EventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var ob = checkbox.BindingContext as filteredTailNumber;
            if (ob.isChecked != true)
            {
                filtertails = filtertails +","+ ob.TailNumber;
            }
        }

        private void btnCancelPop_Tapped(object sender, EventArgs e)
        {
            PopupNavigation.RemovePageAsync(this);
        }

        private void btnFilter_Tapped(object sender, EventArgs e)
        {
            Services.Settings.FilterItems = null;
            Services.Settings.FilterItems = filtertails;
            try
            {
                MessagingCenter.Send<App>((App)Application.Current, "OnFilterSelected");
            }
            catch (Exception) { }

            PopupNavigation.RemovePageAsync(this);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = txtSearchBar.Text;
            MultiSelectListView.ItemsSource = getTailNumbers().Where(x=>x.TailNumber.ToLower().Contains(keyword.ToLower()));
        }

        private void btnSelectAll_Clicked(object sender, EventArgs e)
        {
            MultiSelectListView.ItemsSource = getTailNumbers();
        }


    }
}

