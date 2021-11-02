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

        string filtertails = "";
        string AllTails = "";

        public FlightFilterPopup()
        {
            preflightServices = new PreflightServices();
            InitializeComponent();
            BindingContext = this;
            Loader.IsVisible = false;

            filtertails = Services.Settings.FilterItems;
        }

        public ObservableCollection<filteredTailNumber> FilteredTailNumbers { get => getTailNumbers(); }


        public ObservableCollection<filteredTailNumber> getTailNumbers()
        {
            string fi = "";
            string[] filterItemsArray= { };

            var filteredTails = preflightServices.getTailnumbers();

            List<filteredTailNumber> TailList = new List<filteredTailNumber>();

            if (flag==0)
            {

                if (Services.Settings.FilterItems != null) // && Services.Settings.FilterItems != ""
                {
                    try
                    {
                        fi = Services.Settings.FilterItems.Remove(0, 1);
                        filterItemsArray = fi.Split(',',' ');
                    }
                    catch { }

                    
                        foreach (var item in filteredTails)
                        {
                            AllTails = AllTails + "," + item.tailNumber;

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
            else if (flag == 1)
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


        

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (flag==1)//selected All
            {
                btnSelectAll.Text = "Deselect All";
                filtertails = "";
            }
            else if(flag==2)//Deselected All
            {
                btnSelectAll.Text = "Select All";
                filtertails = AllTails;
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
                else
                {
                    filtertails = filtertails.Replace( "," + ob.TailNumber,"");
                }
            }
        }

        private bool isBusy=false;
        public bool IsBusy { get { return isBusy; } set { isBusy = value; OnPropertyChanged(); } }


        private async void btnFilter_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            Services.Settings.FilterItems = filtertails; //.Remove(0,',')
            try
            {
                MessagingCenter.Send<App>((App)Application.Current, "OnFilterSelected");
            }
            catch (Exception) { }
            
            //PopupNavigation.RemovePageAsync(this);
            await PopupNavigation.Instance.PopAsync();
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

        private async void btnCancelPop_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }

    public class filteredTailNumber
    {
        public string TailNumber { get; set; }
        public string serialNumber { get; set; }

        public bool isChecked { get; set; }
    }

}

