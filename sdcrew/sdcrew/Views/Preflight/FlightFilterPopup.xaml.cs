using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using sdcrew.Services.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using input = Plugin.InputKit.Shared.Controls;

namespace sdcrew.Views.Preflight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class FlightFilterPopup : PopupPage, INotifyPropertyChanged
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

            txtSearchBar.TextChanged += txtSearchBar_TextChanged;

            //MultiSelectListView.ItemsSource = getTailNumbers();
        }

        public ObservableCollection<filteredTailNumber> FilteredTailNumbers { get => getTailNumbers(); }

        public ObservableCollection<filteredTailNumber> getTailNumbers()
        {
            List<filteredTailNumber> TailList = new List<filteredTailNumber>();

            string fi = "";
            string[] filterItemsArray = { };

            if (txtSearchBar.Text != "" & txtSearchBar.Text != null)
            {
                try
                {
                    string keyword = txtSearchBar.Text ?? "";
                    var lst = preflightServices.getTailnumbers().Where(x => x.tailNumber.ToLower().Contains(keyword.ToLower()) || x.serialNumber.Contains(keyword)).ToList();


                    foreach (var item in lst)
                    {
                        TailList.Add(new filteredTailNumber
                        {
                            tailNumber = item.tailNumber,
                            serialNumber = "SN: " + item.serialNumber,
                            isChecked = false,
                        });
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }
            else
            {
                var filteredTails = preflightServices.getTailnumbers();

                if (flag == 0)
                {

                    if (Services.Settings.FilterItems != null) // && Services.Settings.FilterItems != ""
                    {
                        try
                        {
                            fi = Services.Settings.FilterItems.Remove(0, 1);
                            filterItemsArray = fi.Split(',', ' ');
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
                                    tailNumber = item.tailNumber,
                                    serialNumber = "SN: " + item.serialNumber,
                                    isChecked = false
                                });
                            }
                            else
                            {
                                TailList.Add(new filteredTailNumber
                                {
                                    tailNumber = item.tailNumber,
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
                                tailNumber = item.tailNumber,
                                serialNumber = "SN: " + item.serialNumber,
                                isChecked = true
                            });
                        }
                    }
                }
                else if (flag == 1)
                {
                    btnSelectAll.Text = "Deselect All";

                    foreach (var item in filteredTails)
                    {
                        TailList.Add(new filteredTailNumber
                        {
                            tailNumber = item.tailNumber,
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
                            tailNumber = item.tailNumber,
                            serialNumber = "SN: " + item.serialNumber,
                            isChecked = false
                        });
                    }
                }


            }

            var TailCollection = new ObservableCollection<filteredTailNumber>(TailList);
            return TailCollection;
        }


        private void btnSelectAll_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (btnSelectAll.Text == "Deselect All")
                {
                    flag = 2;
                    OnPropertyChanged(nameof(FilteredTailNumbers));
                    filtertails = AllTails;

                }
                else
                {
                    flag = 1;
                    OnPropertyChanged(nameof(FilteredTailNumbers));
                    filtertails = "";
                }
            }
            catch (Exception exc)
            {
                Console.Write(exc);
            }
        }

        private async void btnCancelPop_Tapped(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        private void txtSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FilteredTailNumbers));
        }

        private async void btnFilter_Tapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => Loader.IsVisible = true);

            await Task.Run(() =>
            {
                Services.Settings.FilterItems = filtertails; //.Remove(0,',')
                try
                {
                    MessagingCenter.Send<App>((App)Application.Current, "OnFilterSelected");
                }
                catch (Exception) { }
            });
            await PopupNavigation.Instance.PopAsync();
        }

        private void btntxtSearch_Tapped(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(FilteredTailNumbers));
        }

        void CheckBox_CheckChanged(System.Object sender, System.EventArgs e)
        {
            btnSelectAll.Text = "Deselect All";
            flag = 0;

            var checkbox = (input.CheckBox)sender;
            var ob = checkbox.BindingContext as filteredTailNumber;
            if (ob.isChecked != true)
            {
                filtertails = filtertails + "," + ob.tailNumber;
            }
            else
            {
                filtertails = filtertails.Replace("," + ob.tailNumber, "");
            }
        }

        void TailCell_Tapped(System.Object sender, System.EventArgs e)
        {
            
        }
    }

    public class filteredTailNumber
    {
        public string tailNumber { get; set; }
        public string serialNumber { get; set; }

        public bool isChecked { get; set; }
    }

}

