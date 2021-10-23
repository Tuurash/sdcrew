using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Models;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;




namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlightFilterPopup : PopupPage,INotifyPropertyChanged
    {
        private readonly PreflightServices preflightServices;

        filteredTailNumber filteredTail = new filteredTailNumber();

        int flag = 0;

        public FlightFilterPopup()
        {
            preflightServices = new PreflightServices();
            InitializeComponent();
            BindingContext = this;
        }

        public ObservableCollection<filteredTailNumber> FilteredTailNumbers { get => getTailNumbers(); }


        public ObservableCollection<filteredTailNumber> getTailNumbers()
        {
            var filteredTails = preflightServices.getTailnumbers();

            List<filteredTailNumber> TailList = new List<filteredTailNumber>();

            if(flag==0)
            {

                if (Services.Settings.FilterItems != null && Services.Settings.FilterItems != "")
                {
                    string fi = Services.Settings.FilterItems.Remove(0, 1);
                    string[] filterItemsArray = fi.Split(',');

                    foreach (var item in filteredTails)
                    {
                        bool has = filterItemsArray.Any(x => x.Contains(item.tailNumber));
                        if (has == true)
                        {
                            TailList.Add(new filteredTailNumber
                            {
                                TailNumber = item.tailNumber,
                                serialNumber = "SN: " + item.serialNumber,
                                isChecked = false
                            });
                        }
                        else
                        {
                            TailList.Add(new filteredTailNumber
                            {
                                TailNumber = item.tailNumber,
                                serialNumber = "SN: " + item.serialNumber,
                                isChecked = true
                            });
                        }
                    }
                }
                else
                {
                    btnSelectAll.Text = "Deselect All";
                    foreach (var item in filteredTails)
                    {
                        TailList.Add(new filteredTailNumber
                        {
                            TailNumber = item.tailNumber,
                            serialNumber = "SN: " + item.serialNumber,
                            isChecked = true
                        });
                    }
                }
            }
            else if(flag==1)
            {
                foreach (var item in filteredTails)
                {
                    TailList.Add(new filteredTailNumber
                    {
                        TailNumber = item.tailNumber,
                        serialNumber = "SN: " + item.serialNumber,
                        isChecked = true
                    });
                }
            }
            else if (flag == 2)
            {
                btnSelectAll.Text = "Select All";
                foreach (var item in filteredTails)
                {
                    TailList.Add(new filteredTailNumber
                    {
                        TailNumber = item.tailNumber,
                        serialNumber = "SN: " + item.serialNumber,
                        isChecked = false
                    });
                }
            }

            var TailCollection = new ObservableCollection<filteredTailNumber>(TailList);
            return TailCollection;
        }


        string filtertails = "";

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {

            if(flag==1)
            {
                btnSelectAll.Text = "Deselect All";
                Services.Settings.FilterItems = "";
            }
            else if(flag==2)
            {
                btnSelectAll.Text = "Select All";



            }
            else
            {
                btnSelectAll.Text = "Deselect All";
                var checkbox = (CheckBox)sender;
                var ob = checkbox.BindingContext as filteredTailNumber;
                if (ob.isChecked != true)
                {
                    filtertails = filtertails + "," + ob.TailNumber;
                }
            }
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
            MultiSelectListView.ItemsSource = getTailNumbers().Where(x => x.TailNumber.ToLower().Contains(keyword.ToLower()));
        }

        private void btnSelectAll_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (btnSelectAll.Text == "Deselect All")
                {
                    flag = 2;
                    OnPropertyChanged(nameof(FilteredTailNumbers));
                }
                else
                {
                    flag = 1;
                    OnPropertyChanged(nameof(FilteredTailNumbers));
                }
            }catch(Exception exc)
            {
                Console.Write(exc);
            }
        }

        private void btnCancelPop_Tapped(object sender, EventArgs e)
        {
            PopupNavigation.RemovePageAsync(this);
        }
    }

    public class filteredTailNumber
    {
        public string TailNumber { get; set; }
        public string serialNumber { get; set; }

        public bool isChecked { get; set; }
    }

}

